using UnityEngine;

namespace Environnement
{
    public class GridSetter : MonoBehaviour
    {
        private GridManager grid;
        public int objectLength;
        public int objectWidth;

        private void Awake()
        {
            grid = GameObject.FindGameObjectWithTag("GridManager").GetComponent<GridManager>();
        }

        private void Start()
        {
            grid.SetValue(transform.position, -1);
            int x, y;
            grid.getXY(transform.position, out x, out y);
        
            if (transform.rotation.y == 0 || transform.rotation.y == 1 || transform.rotation.y == -1)
            {
                grid.SetValue(x + 1, y, -1);
                grid.SetValue(x - 1, y, -1);
            }
            else
            {
                grid.SetValue(x, y + 1, -1);
                grid.SetValue(x, y - 1, -1);
            }

            for (int i = 1; i <= objectLength; i++)
            {
                if (transform.rotation.y == 0 || transform.rotation.y == 1 || transform.rotation.y == -1)
                {
                    grid.SetValue(x, y + i, -1);
                    grid.SetValue(x, y - i, -1);
                }
                else
                {
                    grid.SetValue(x + i, y, -1);
                    grid.SetValue(x - i, y, -1);
                }

                for (int j = 1; j <= objectWidth; j++)
                {
                    if (transform.rotation.y == 0 || transform.rotation.y == 1 || transform.rotation.y == -1)
                    {
                        grid.SetValue(x + j, y + i, -1);
                        grid.SetValue(x - j, y + i, -1);
                        grid.SetValue(x + j, y - i, -1);
                        grid.SetValue(x - j, y - i, -1);
                    }
                    else
                    {
                        grid.SetValue(x + i, y + j, -1);
                        grid.SetValue(x - i, y + j, -1);
                        grid.SetValue(x + i, y - j, -1);
                        grid.SetValue(x - i, y - j, -1);
                    }
                }
            }
        }
    }
}
