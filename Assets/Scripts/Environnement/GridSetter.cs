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
            print(gameObject);
            print(gameObject.transform.position);

            if (objectWidth > 0)
            {
                for (int i = 1; i <= objectWidth; i++)
                {
                    if ((int) transform.rotation.eulerAngles.y == 0 || (int) transform.rotation.eulerAngles.y == 180 ||
                        (int) transform.rotation.eulerAngles.y == -180)
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
                if ((int)transform.rotation.eulerAngles.y == 0 || (int)transform.rotation.eulerAngles.y == 180 || (int)transform.rotation.eulerAngles.y == -180)
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
                    if ((int)transform.rotation.eulerAngles.y == 0 || (int)transform.rotation.eulerAngles.y == 180 || (int)transform.rotation.eulerAngles.y == -180)
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
    }
}
