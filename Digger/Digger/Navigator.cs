using System;
using Digger.Views.Common;

namespace Digger
{
    /// <summary>
    /// Klasa umożliwiająca wykonywanie operacji związanych z nawigacją między widokami w grze.
    /// </summary>
    public static class Navigator
    {
        /// <summary>
        /// Funkcja wykonująca operację przejścia do wskazanego widoku.
        /// </summary>
        /// <param name="type">Typ klasy okna, do której ma zostać wykonane przejście.</param>
        /// <param name="arg">Opcjonalny, umożliwia ustawienie stanu wczytywanego obiektu.</param>
        public static void NavigateTo(Type type, int? arg = null)
        {
            if (arg != null)
            {
                Window.Context.CurrentView = (IXnaUseable)Activator.CreateInstance(type, arg.Value);
                return;
            }
            Window.Context.CurrentView = (IXnaUseable)Activator.CreateInstance(type);
        }
    }
}