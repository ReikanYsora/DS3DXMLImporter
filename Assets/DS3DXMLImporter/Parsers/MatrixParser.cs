using System.Globalization;
using UnityEngine;

namespace DS3DXMLImporter.Parsers
{
    public class MatrixParser
    {
        #region ATTRIBUTES
        private float _xx;
        private float _xy;
        private float _xz;
        private float _yx;
        private float _yy;
        private float _yz;
        private float _zx;
        private float _zy;
        private float _zz;
        private float _tx;
        private float _ty;
        private float _tz;
        #endregion

        #region PROPERTIES
        public Vector3 Position { private set; get; }

        public Quaternion Rotation { private set; get; }
        #endregion


        #region CONSTRUCTOR
        public MatrixParser(string matrixStr)
        {
            string position = string.Empty;
            string rotation = string.Empty;
            matrixStr = matrixStr.Replace(",", ".");
            string[] matrixMembers = matrixStr.Split(' ');
            int i = 0;

            for (i = 0; i < 9; i++)
            {
                rotation += matrixMembers[i] + " ";
            }

            rotation = rotation.Remove(rotation.Length - 1);

            for (i = 9; i < 12; i++)
            {
                position += matrixMembers[i] + " ";
            }

            position = position.Remove(position.Length - 1);

            string[] positionTokens = position.Split(' ');

            if (positionTokens.Length == 3)
            {
                float.TryParse(positionTokens[0], NumberStyles.Float, CultureInfo.InvariantCulture, out _tx);
                float.TryParse(positionTokens[1], NumberStyles.Float, CultureInfo.InvariantCulture, out _ty);
                float.TryParse(positionTokens[2], NumberStyles.Float, CultureInfo.InvariantCulture, out _tz);
            }

            string[] rotationsStr = rotation.Split(' ');

            if (rotationsStr.Length == 9)
            {
                float.TryParse(rotationsStr[0], NumberStyles.Float, CultureInfo.InvariantCulture, out _xx);
                float.TryParse(rotationsStr[1], NumberStyles.Float, CultureInfo.InvariantCulture, out _xy);
                float.TryParse(rotationsStr[2], NumberStyles.Float, CultureInfo.InvariantCulture, out _xz);
                float.TryParse(rotationsStr[3], NumberStyles.Float, CultureInfo.InvariantCulture, out _yx);
                float.TryParse(rotationsStr[4], NumberStyles.Float, CultureInfo.InvariantCulture, out _yy);
                float.TryParse(rotationsStr[5], NumberStyles.Float, CultureInfo.InvariantCulture, out _yz);
                float.TryParse(rotationsStr[6], NumberStyles.Float, CultureInfo.InvariantCulture, out _zx);
                float.TryParse(rotationsStr[7], NumberStyles.Float, CultureInfo.InvariantCulture, out _zy);
                float.TryParse(rotationsStr[8], NumberStyles.Float, CultureInfo.InvariantCulture, out _zz);
            }

            Position = new Vector3(_tx, _ty, _tz);
            Quaternion tempRotation = Quaternion.LookRotation(new Vector3(_zx, _zy, _zz), new Vector3(_yx, _yy, _yz));
            Rotation = new Quaternion(tempRotation.x, tempRotation.z, tempRotation.y, -tempRotation.w);

            positionTokens = null;
            rotationsStr = null;
        }
        #endregion
    }
}
