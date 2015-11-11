using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


public static class SqlCommandExtension
{
    /// <summary>
    /// This will add an array of parameters to a SqlCommand. This is used for an IN statement.
    /// Use the returned value for the IN part of your SQL call. (i.e. SELECT * FROM table WHERE field IN ({paramNameRoot}))
    /// </summary>
    /// <param name="cmd">The SqlCommand object to add parameters to.</param>
    /// <param name="values">The array of strings that need to be added as parameters.</param>
    /// <param name="paramNameRoot">What the parameter should be named followed by a unique value for each value. This value surrounded by {} in the CommandText will be replaced.</param>
    /// <param name="start">The beginning number to append to the end of paramNameRoot for each value.</param>
    /// <param name="separator">The string that separates the parameter names in the sql command.</param>
    public static SqlParameter[] AddArrayParameters<T>(this SqlCommand cmd, IEnumerable<T> values, string paramNameRoot, int start = 1, string separator = ", ")
    {
        /* An array cannot be simply added as a parameter to a SqlCommand so we need to loop through things and add it manually. 
            * Each item in the array will end up being it's own SqlParameter so the return value for this must be used as part of the
            * IN statement in the CommandText.
            */
        var parameters = new List<SqlParameter>();
        var parameterNames = new List<string>();
        var paramNbr = start;
        foreach (var value in values)
        {
            var paramName = string.Format("@{0}{1}", paramNameRoot, paramNbr++);
            parameterNames.Add(paramName);
            parameters.Add(cmd.Parameters.AddWithValue(paramName, value));
        }

        cmd.CommandText = cmd.CommandText.Replace("{" + paramNameRoot + "}", string.Join(separator, parameterNames));

        return parameters.ToArray();
    }
    public static SqlParameter[] AddObjectsParamaters(this SqlCommand cmd, IEnumerable<IDictionary<string, object>> objects, string paramNameRoot, int start = 1, string separator = ", ")
    {
        var pattern = string.Format(@"\{{{0}:(.*?)\}}", paramNameRoot);
        var parameters = new List<SqlParameter>();
        var baseMatch = Regex.Match(cmd.CommandText, pattern);
        if (!baseMatch.Success)
            return parameters.ToArray();

        var groupBase = baseMatch.Groups[1].Value;
        var paramMatch = Regex.Match(groupBase, @"@([a-z|A-Z|0-9]+)");
        var paramNbr = start;
        var groups = new List<string>();
        foreach (var obj in objects)
        {
            foreach (KeyValuePair<string, object> param in obj)
            {
                var paramName = string.Format("@{0}{1}", paramNameRoot, paramNbr++);
                var group = groupBase.Replace(string.Format("@{0}", param.Key), paramName);
                groups.Add(group);
                parameters.Add(cmd.Parameters.AddWithValue(paramName, param.Value));
            }
        }
        cmd.CommandText = Regex.Replace(cmd.CommandText, pattern, string.Join(separator, groups));

        return parameters.ToArray();
    }
}
