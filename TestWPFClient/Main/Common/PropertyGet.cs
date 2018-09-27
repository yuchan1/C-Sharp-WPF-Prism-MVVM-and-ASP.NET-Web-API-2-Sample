using System;
using System.Linq.Expressions;

namespace Main.Common {
    public static class PropertyGet {

        /// <summary>
        /// GetPropertyName is nameof substitution(C#5.0 not nameof)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetPropertyName<T>(Expression<Func<T>> e) {
            var memberEx = (MemberExpression)e.Body;
            return memberEx.Member.Name;
        }
    }
}
