using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CivClone
{
    /// <summary>
    /// Represents a simple vertical menu with selectable options.
    /// MoveUp and MoveDown cycle through the options; SelectedOption
    /// returns the currently highlighted option.
    /// </summary>
    public class Menu
    {
        public string[] Options { get; }
        private int _selectedIndex;

        public Menu(string[] options)
        {
            Options = options;
            _selectedIndex = 0;
        }

        public string SelectedOption => Options[_selectedIndex];

        public void MoveUp()
        {
            _selectedIndex--;
            if (_selectedIndex < 0) _selectedIndex = Options.Length - 1;
        }

        public void MoveDown()
        {
            _selectedIndex++;
            if (_selectedIndex >= Options.Length) _selectedIndex = 0;
        }

        /// <summary>
        /// Draws the menu on screen using the provided SpriteBatch and font.
        /// The selected option is drawn in yellow; others in white.  If
        /// font is null nothing is drawn.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, SpriteFont? font, Vector2 position)
        {
            if (font == null) return;
            for (int i = 0; i < Options.Length; i++)
            {
                var color = i == _selectedIndex ? Color.Yellow : Color.White;
                spriteBatch.DrawString(font, Options[i], new Vector2(position.X, position.Y + i * (font.LineSpacing + 5)), color);
            }
        }
    }
}