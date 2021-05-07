using UnityEngine;

namespace Environnement
{
    public class GridSetter : MonoBehaviour
    {
        private GridManager grid;
        [SerializeField]private bool pivot;

        public int objectLength;
        public int objectWidth;


        public bool Pivot
        {
            get => pivot;
            set => pivot = value;
        }

        private void Awake()
        {
            grid = GameObject.FindGameObjectWithTag("GridManager").GetComponent<GridManager>();
        }

        private void Start()
        {
            grid.SetValue(transform.position, -1);
            int x, y;
            grid.getXY(transform.position, out x, out y);
            int rotation = NormalizedRotation();

            if (objectWidth > 0)
            {
                for (int i = 1; i <= objectWidth; i++)
                {
                    if (rotation == 0 || rotation == 180 || rotation == -180)
                    {
                        if (pivot)
                        {
                            grid.SetValue(x, y + i, -1);
                            grid.SetValue(x, y - i, -1);
                        }
                        else
                        {
                            grid.SetValue(x + i, y, -1);
                            grid.SetValue(x - i, y, -1);
                        }
                    }
                    else
                    {
                        if (pivot)
                        {
                            grid.SetValue(x + i, y, -1);
                            grid.SetValue(x - i, y, -1);
                        }
                        else
                        {
                            grid.SetValue(x, y + i, -1);
                            grid.SetValue(x, y - i, -1);
                        }
                    }
                }
            }

            for (int i = 1; i <= objectLength; i++)
            {
                if (rotation == 0 || rotation == 180 || rotation == -180)
                {
                    if (pivot)
                    {
                        grid.SetValue(x + i, y, -1);
                        grid.SetValue(x - i, y, -1);
                    }
                    else
                    {
                        grid.SetValue(x, y + i, -1);
                        grid.SetValue(x, y - i, -1);
                    }
                }
                else
                {
                    if (pivot)
                    {
                        grid.SetValue(x, y + i, -1);
                        grid.SetValue(x, y - i , -1);
                    }
                    else
                    {
                        grid.SetValue(x + i, y, -1);
                        grid.SetValue(x - i, y, -1);
                    }
                }
                
                for (int j = 1; j <= objectWidth; j++)
                {
                    if (rotation == 0 || rotation == 180 || rotation == -180)
                    {
                        if (pivot)
                        {
                            grid.SetValue(x + i, y + j, -1);
                            grid.SetValue(x - i, y + j, -1);
                            grid.SetValue(x + i, y - j, -1);
                            grid.SetValue(x - i, y - j, -1);
                        }
                        else
                        {
                            grid.SetValue(x + j, y + i, -1);
                            grid.SetValue(x - j, y + i, -1);
                            grid.SetValue(x + j, y - i, -1);
                            grid.SetValue(x - j, y - i, -1);
                        }
                    }
                    else
                    {
                        if (pivot)
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

        private int NormalizedRotation()
        {
            int rotation = (int)transform.rotation.eulerAngles.y;
            if (rotation % 10 != 0)
            {
                rotation += 1;
            }
            if (rotation % 10 != 0)
            {
                rotation -= 2;
            }

            return rotation;

            // if (rotation > -45 && rotation < 45)
            // {
            //     rotation = 0;
            // }
            //
            // if (rotation > 45 && rotation < 135)
            // {
            //     rotation = 90;
            // }
            //
            // if (rotation > 135 && rotation < 225)
            // {
            //     rotation = 180;
            // }
            //
            // if (rotation > 225 && rotation < 315)
            // {
            //     rotation = 270;
            // }
            //
            // if (rotation < -45 && rotation )
        }
    }
}
