using System;
using Digger.Views.Common;

namespace Digger
{
    public static class Navigator
    {
        public static void NavigateTo(Type type)
        {
            Game1.Context.CurrentView = (IXnaUseable)Activator.CreateInstance(type);
        }
    }
}