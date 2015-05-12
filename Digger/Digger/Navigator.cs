using System;
using Digger.Views.Common;

namespace Digger
{
    public static class Navigator
    {
        public static void NavigateTo(Type type, int? arg = null)
        {
            if (arg != null)
            {
                Game1.Context.CurrentView = (IXnaUseable)Activator.CreateInstance(type, arg.Value);
                return;
            }
            Game1.Context.CurrentView = (IXnaUseable)Activator.CreateInstance(type);
        }
    }
}