using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using SyntaxTree.VisualStudio.Unity.Bridge;
using UnityEngine;

[InitializeOnLoad]
public class SolutionGenerationHook
{

    static SolutionGenerationHook()
    {
        ProjectFilesGenerator.SolutionFileGeneration += (name, content) =>
        {
            try
            {
                string assemblyName = ExternalProjectConfiguration.Instance["assemblyName"];
                string projectFilePath = ExternalProjectConfiguration.Instance["projectFilePath"];
                string projectGuid = ExternalProjectConfiguration.Instance["projectGuid"];

                content = AddProjectToSolution(content, assemblyName, projectFilePath, projectGuid);

                //Debug.Log("SolutionGenerationHook:" + name);
                return content;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return null;
        };
    }

    private static string AddProjectToSolution(string content, string projectName, string projectFilePath, string projectGuid)
    {
        if (content.Contains("" + projectName + ""))
            return content; // already added

        var signature = new StringBuilder();
        string csharpProjectTypeGuid = ExternalProjectConfiguration.Instance["projectGuid"];
        signature.AppendLine(string.Format("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"", csharpProjectTypeGuid, projectName, projectFilePath, projectGuid));
        signature.AppendLine("Global");

        var regex = new Regex("^Global", RegexOptions.Multiline);
        return regex.Replace(content, signature.ToString());
    }
}
