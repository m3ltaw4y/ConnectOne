//Menuless Scriptable Object Creator Window
//Please see ZixBoy/MenulessScriptableObjectCreator/Documentation/readme.txt in Assets
//Or visit ZixBoy.com for more information.
//Email Support@ZixBoy.com with any issues
//Version 1.0. 2021 Jan 22

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZixBoy.MenulessScriptableObjectCreator.Editor
{
    public class MenulessScriptableObjectCreator : EditorWindow
    {
        [MenuItem("Window/Menuless Scriptable Object Creator")]
        public static void ShowWindow()
        {
            GetWindow(typeof(MenulessScriptableObjectCreator), false,"Scriptable Object Creator");
        }

        private Type[] possibleTypes; //All possible types in the search assemblies
        private string searchString = string.Empty; //What is currently being searched for
        private bool any; //if this is set, every assembly in the project will be searched
        private Vector2 scrollPos = Vector2.zero; //position of upper window scroll 
        private Vector2 lowerScrollPos = Vector2.zero; //position of lower window scroll 
        private bool showAssemblies; //expand or collapse the assemble list

        //A helper class to help show and select searchable assemblies.
        [Serializable]
        public class SearchedAssembly
        {
            public string assemblyName;//the displayed name
            public bool search;//to search this assembly for scriptable objects, or not

            public SearchedAssembly(string assemblyName, bool search = false)
            {
                this.assemblyName = assemblyName;
                this.search = search;
            }
        }

        //A list of all assemblies, comprised of their names and if they are being searched
        [SerializeField] public List<SearchedAssembly> assembliesToSearch;

        private UnityEditor.Editor editor;

        private void OnGUI()
        {
            assembliesToSearch = assembliesToSearch ?? new List<SearchedAssembly>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();//gets all the assemblies in the project
            if (assembliesToSearch.Count != assemblies.Length)//a refresh is required in this case
            {
                assembliesToSearch.Clear();
                //populate the list of assemblies
                foreach (var assembly in assemblies)
                {
                    var asmName = assembly.GetName().Name;
                    //"Assembly-CSharp" is the only one which is searched by default
                    assembliesToSearch.Add(new SearchedAssembly(asmName, asmName == "Assembly-CSharp"));
                }

                //alphabetize the list
                assembliesToSearch = assembliesToSearch.OrderBy(x => x.assemblyName).ToList();
            }

            List<Assembly> selectedAssemblies;
            //if "any" is set, every assembly in the project will be searched
            if (any)
            {
                selectedAssemblies = assemblies.ToList();
            }
            //otherwise, search only the selected ones
            else
            {
                selectedAssemblies = new List<Assembly>();
                foreach (var assembly in assemblies)
                {
                    foreach (var assemblyToSearch in assembliesToSearch)
                    {
                        if (assemblyToSearch.search && assemblyToSearch.assemblyName == assembly.GetName().Name)
                        {
                            selectedAssemblies.Add(assembly);
                            break;
                        }
                    }
                }
            }

            //Get an array of Types which are ScriptableObjects in the selected assembles.  This is the set of Types to pick from
            possibleTypes = selectedAssemblies
                .SelectMany(a => a.GetTypes())
                .Where(typeof(ScriptableObject).IsAssignableFrom)
                .ToArray();

            //the search bar, at the top of the window
            searchString = GUILayout.TextField(searchString,
#if UNITY_2019
                EditorStyles.toolbarSearchField, //toolbarSearchField doesn't work for unity 2018
#else                
                EditorStyles.textField,         
#endif
                GUILayout.ExpandWidth(true));

            //The section below it, displaying the filtered ScriptableObject Types
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            foreach (var possibleType in possibleTypes)
            {
                //filter by search bar (case insensitive)
                if (possibleType.Name.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) < 0)
                {
                    continue;
                }

                GUILayout.BeginHorizontal();
                // the button to create each asset
                if (GUILayout.Button("Create", GUILayout.MaxWidth(70)))
                {
                    CreateAsset(possibleType);
                }

                //type name
                GUILayout.Label(possibleType.Name);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            //collapsible assemblies selection section
            showAssemblies = EditorGUILayout.Foldout(showAssemblies, "Assemblies to Search");
            lowerScrollPos = GUILayout.BeginScrollView(lowerScrollPos);
            if (showAssemblies)
            {
                foreach (var assembly in assembliesToSearch)
                {
                    //enable searching a specific assembly
                    assembly.search = EditorGUILayout.ToggleLeft(assembly.assemblyName, assembly.search);
                }
            }

            GUILayout.EndScrollView();

            //search all assemblies check box
            any = EditorGUILayout.Toggle("Search All Assemblies", any);
            
            editor = editor == null ? UnityEditor.Editor.CreateEditor(this) : editor;
        }

        //Creates the chosen ScriptableObjectAsset
        private void CreateAsset(Type type)
        {
            //Name the asset after the class name
            var fileName = type.ToString().Split('.').Last();
            var asset = CreateInstance(type);

            string path = "Assets";//default creation location
            //try to create at the selected location, if anything is selected.
            foreach (var obj in Selection.GetFiltered<Object>(SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                if (System.IO.Directory.Exists(path))
                {
                    break;
                }

                if (System.IO.File.Exists(path))
                {
                    path = System.IO.Path.GetDirectoryName(path);
                }
            }
            
            //ensure asset name is unique
            path = AssetDatabase.GenerateUniqueAssetPath(string.Format("{0}/{1}.asset", path, fileName));
            //create it and save!
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = asset;
        }
    }
}