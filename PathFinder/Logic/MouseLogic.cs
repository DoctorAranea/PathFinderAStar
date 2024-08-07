using PathFinder.Logic.GameObjects.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PathFinder.Logic.PFinder;

namespace PathFinder.Logic
{
    public static class MouseLogic
    {
        private static bool mouseDown = false;
        private static Point startPos;

        public static void MouseClicked(PFinder pFinder, object sender, MouseEventArgs e)
        {
            Point clickPosition = e.Location;
            Point startClickPosition = new Point(startPos.X / CELLSIZE, startPos.Y / CELLSIZE);
            Point fieldClickPosition = new Point(clickPosition.X / CELLSIZE, clickPosition.Y / CELLSIZE);

            if (startClickPosition == fieldClickPosition && e.Button == MouseButtons.Left)
            {
                SelectedObjects.Clear();

                Creation selectedCreation = Creations.FirstOrDefault(x => x.FieldPosition == fieldClickPosition);
                if (selectedCreation != null)
                    SelectedObjects.Add(selectedCreation);

                DrawMap();
            }
            else if (SelectedObjects.Count > 0 && e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < SelectedObjects.Count; i++)
                    StartMoving(pFinder, SelectedObjects[i] as Creation, fieldClickPosition);
            }
        }

        public static void StartMoving(PFinder pFinder, Creation creation, Point endPosition)
        {
            if (creation.GizmosPath.Count > 0 && creation.RequestToStop)
                return;

            new Thread(() =>
            {
                creation.RequestToStop = true;
                while (creation.GizmosPath.Count > 0) { Thread.Sleep(1); }
                creation.RequestToStop = false;
                creation.GizmosPath = creation.FindPath(creation.FieldPosition, endPosition);

                while (creation.GizmosPath.Count > 0)
                {
                    bool changedX = creation.FieldPosition.X != creation.GizmosPath[0].X;
                    bool offsetIsNatural = (changedX ? creation.FieldPosition.X : creation.FieldPosition.Y) < (changedX ? creation.GizmosPath[0].X : creation.GizmosPath[0].Y);
                    int offsetsCount = 5;
                    int offset = CELLSIZE / offsetsCount;

                    for (int i = 1; i < offsetsCount; i++)
                    {
                        int totalOffset = offsetIsNatural ? offset : -offset;
                        creation.OffsetX = changedX ? totalOffset * i : 0;
                        creation.OffsetY = changedX ? 0 : totalOffset * i;
                        DrawCreationFromAnotherThread();
                        Thread.Sleep(25);
                    }

                    if (creation.GizmosPath.Count == 0 || creation.RequestToStop)
                    {
                        creation.OffsetX = 0;
                        creation.OffsetY = 0;
                        creation.GizmosPath.Clear();
                        DrawCreationFromAnotherThread();
                        break;
                    }

                    creation.Move(creation.GizmosPath[0]);
                    creation.GizmosPath.RemoveAt(0);

                    creation.OffsetX = 0;
                    creation.OffsetY = 0;
                    DrawCreationFromAnotherThread();

                    void DrawCreationFromAnotherThread()
                    {
                        try
                        {
                            pFinder.Invoke(new Action(() =>
                            {
                                DrawMap();
                            }));
                        }
                        catch { }
                    }
                }
            }).Start();
        }

        internal static void MouseDowned(PFinder pFinder, object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = true;
                startPos = new Point
                (
                    e.Location.X,
                    e.Location.Y
                );
                SelectedArea = new Rectangle(startPos, new Size(1, 1));
            }
        }

        internal static void MouseUpped(PFinder pFinder, object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = false;
                SelectedArea = default;
                DrawMap();
            }
        }

        internal static void MouseMoved(PFinder pFinder, object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Point endPos = new Point
                (
                    e.Location.X,
                    e.Location.Y
                );

                var selectionStartX = endPos.X > startPos.X ? startPos.X : endPos.X;
                var selectionStartY = endPos.Y > startPos.Y ? startPos.Y : endPos.Y;
                var selectionEndX = endPos.X <= startPos.X ? startPos.X : endPos.X;
                var selectionEndY = endPos.Y <= startPos.Y ? startPos.Y : endPos.Y;

                Rectangle selectionRect = new Rectangle()
                {
                    X = selectionStartX,
                    Y = selectionStartY,
                    Width = selectionEndX - selectionStartX + 1,
                    Height = selectionEndY - selectionStartY + 1,
                };

                if (selectionRect != SelectedArea)
                {
                    SelectedArea = selectionRect;

                    SelectedObjects.Clear();

                    Creation[] selectedCreations = Creations.Where(x => SelectedArea.Contains(new Point(x.RealPosition.X + CELLSIZE / 2, x.RealPosition.Y + CELLSIZE / 2))).ToArray();
                    if (selectedCreations != null && selectedCreations.Length > 0)
                        SelectedObjects.AddRange(selectedCreations);

                    DrawMap();
                }
            }
        }
    }
}
