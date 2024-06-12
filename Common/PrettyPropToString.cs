using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common;
public abstract class PrettyPropToString
{
    public int IdentationCount { get; set; }
    public override string ToString()
    {
        // some pretty print shinanigans
        var sb = new StringBuilder();
        System.Reflection.PropertyInfo[] properities = GetType().GetProperties();
        for (int i = 0; i < properities.Length; i++)
        {
            System.Reflection.PropertyInfo? prop = properities[i];
            if (prop.Name == nameof(IdentationCount)) continue;

            var value = prop.GetValue(this, null);
            if (value != null && value is PrettyPropToString child)
            {
                child.IdentationCount = IdentationCount + 1;
                sb.Append(new string('\t', IdentationCount));
                sb.Append(prop.Name);
                sb.Append(": ");
                sb.Append('\n');
            }
            else
            {
                sb.Append(new string('\t', IdentationCount));
                sb.Append(prop.Name);
                sb.Append(": ");
            }
            sb.Append(value);
            // don't print newline after the last prop (-2 instead of -1 because we are ignoring the identationCount prop)
            if (i != properities.Length - 2) sb.Append('\n');
        }
        return sb.ToString();
    }
}
