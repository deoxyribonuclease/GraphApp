using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp8
{
    public partial class MainWindow : Window
    {
        // Змінні для збереження списків вершин та ребер, а також іншої необхідної інформації
        private List<Vertex> vertices; // список вершин
        private List<Edge> edges; // список ребер
        private bool isDragging; // прапорець для перетягування вершини
        private bool isCreatingEdge; // прапорець для створення ребра
        private Vertex selectedVertex; // обрана вершина
        private Line edgeLine; // лінія для показу майбутнього ребра

        public MainWindow()
        {
            InitializeComponent();
            // Ініціалізуємо змінні
            vertices = new List<Vertex>();
            edges = new List<Edge>();
            isDragging = false;
            isCreatingEdge = false;
            selectedVertex = null;
            edgeLine = null;
            label.Content = $"Selected Vertex: None\nPosition (X:{0} Y:{0})";
        }

        // Метод, який викликається при натисканні на кнопку Add Vertex
        private void AddVertexButton_Click(object sender, RoutedEventArgs e)
        {
            CreateVertex();
        }

        // Метод для створення вершини
        private void CreateVertex()
        {
            // Змінна, що визначає номер вершини
            int vertexNumber = vertices.Count + 1;
            // Створення нового об'єкта вершини
            var obj = new Vertex(vertexNumber);
            // Поліморфна змінна типу GraphElement, яка посилається на об'єкт вершини
            GraphElement vertex = obj;
            // Прикріплення обробників подій до вершини
            vertex.MouseDown += Vertex_MouseDown;
            vertex.MouseMove += Vertex_MouseMove;
            vertex.MouseUp += Vertex_MouseUp;
            // Встановлення позиції вершини на Canvas
            Canvas.SetLeft(vertex, 100);
            Canvas.SetTop(vertex, 100);
            // Додавання вершини до колекції на Canvas
            graphCanvas.Children.Add(vertex);
            // Додавання вершини до колекції vertices
            vertices.Add((Vertex)vertex);
            // Оновлення матриці суміжності
            AdjacencyMatrix();
        }

        // Обробник події натискання мишею на вершину
        private void Vertex_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isCreatingEdge)
            {
                selectedVertex = sender as Vertex;
                StartEdgeCreation();
            }
            else
            {
                selectedVertex = sender as Vertex;
                isDragging = true;
                selectedVertex.CaptureMouse();
            }
        }

        // Обробник події руху миші над вершиною
        private void Vertex_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                // Отримання нових координат вершини
                double newX = e.GetPosition(graphCanvas).X - (selectedVertex.ActualWidth / 2);
                double newY = e.GetPosition(graphCanvas).Y - (selectedVertex.ActualHeight / 2);

                // Перевірка, чи вершина не виходить за межі Canvas
                double canvasWidth = graphCanvas.ActualWidth;
                double canvasHeight = graphCanvas.ActualHeight;

                if (newX < 0)
                    newX = 0;
                else if (newX + selectedVertex.ActualWidth > canvasWidth)
                    newX = canvasWidth - selectedVertex.ActualWidth;

                if (newY < 0)
                    newY = 0;
                else if (newY + selectedVertex.ActualHeight > canvasHeight)
                    newY = canvasHeight - selectedVertex.ActualHeight;

                label.Content = $"Selected Vertex: {selectedVertex.Content}\nPosition (X:{newX} Y:{newY})";
                // Зміна позиції вершини
                Canvas.SetLeft(selectedVertex, newX);
                Canvas.SetTop(selectedVertex, newY);
                UpdateEdgePositions(selectedVertex);
            }
        }



        // Обробник події відпускання кнопки миші після перетягування вершини
        private void Vertex_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isCreatingEdge)
            {
                var targetVertex = GetVertexUnderMouse(e.GetPosition(graphCanvas));
                if (targetVertex != null && targetVertex != selectedVertex)
                {
                    CreateEdge(selectedVertex, targetVertex);
                }
                StopEdgeCreation();
            }
            else if (isDragging)
            {
                isDragging = false;
                selectedVertex.ReleaseMouseCapture();
            }
            AdjacencyMatrix();
        }

        // Клас, що містить методи для роботи з ребрами та їх створення
        private void Edge_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isCreatingEdge) return;

            var clickedEdge = sender as Edge;
            RemoveEdge(clickedEdge);
        }

        // Створення ребра між двома вершинами
        private void CreateEdge(Vertex startVertex, Vertex endVertex)
        {
            // Створюємо нове ребро з початковою та кінцевою вершинами
            var edge = new Edge(startVertex, endVertex); 
            // Додано обробник події кліку мишею на ребро
            edge.MouseDown += Edge_MouseDown;
            // Додаємо ребро до колекції ребер
            edges.Add(edge);
            // Додаємо ребро до графічного полотну
            graphCanvas.Children.Add(edge);
            // Оновлюємо позиції ребер для початкової вершини
            UpdateEdgePositions(startVertex); 
        }

        // Початок створення ребра (перша вершина вибрана)
        private void StartEdgeCreation()
        {
            // Створюємо новий об'єкт лінії
            edgeLine = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                X1 = selectedVertex.CenterX,
                Y1 = selectedVertex.CenterY,
                X2 = selectedVertex.CenterX,
                Y2 = selectedVertex.CenterY
            };
            // Додаємо лінію до графічного канвасу
            graphCanvas.Children.Add(edgeLine);
        }

        // Зупинка створення ребра 
        private void StopEdgeCreation()
        {
            //якщо вже була створена лінія
            if (edgeLine != null)
            {
                // Видаляємо лінію з графічного канвасу
                graphCanvas.Children.Remove(edgeLine);
                // Звільняємо пам'ять, присвоюючи значення null
                edgeLine = null;
            }
        }

        // Оновлення позицій ребер, які з'єднані з заданою вершиною
        public void UpdateEdgePositions(Vertex vertex)
        {
            foreach (var edge in edges)
            {
                if (edge.StartVertex == vertex || edge.EndVertex == vertex)
                {
                    edge.UpdatePosition();
                }
            }
        }

        // Отримання положення вершини 
        private Vertex GetVertexUnderMouse(Point position)
        {
            foreach (var vertex in vertices)
            {
                if (IsMouseOverVertex(position, vertex))
                {
                    return vertex;
                }
            }
            return null;
        }
        // Перевірка, чи знаходиться курсор миші над заданою вершиною
        private bool IsMouseOverVertex(Point position, Vertex vertex)
        {
            double left = Canvas.GetLeft(vertex);
            double top = Canvas.GetTop(vertex);
            double right = left + vertex.ActualWidth;
            double bottom = top + vertex.ActualHeight;

            return (position.X >= left && position.X <= right && position.Y >= top && position.Y <= bottom);
        }

        // Обробка події натиснення кнопки видалення вершини
        private void RemoveVertexButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelectedVertex();
        }

        // Видалення вибраної вершини
        private void RemoveSelectedVertex()
        {
            if (selectedVertex != null)
            {
                // Видалення ребер, що з'єднуються з вибраною вершиною
                RemoveEdgesConnectedToVertex(selectedVertex);
                // Видалення вибраної вершини з графічного контейнера
                graphCanvas.Children.Remove(selectedVertex);
                // Знаходження індексу видаленої вершини в списку вершин
                int removedIndex = vertices.IndexOf(selectedVertex);
                // Видалення вершини зі списку вершин
                vertices.RemoveAt(removedIndex);
                // Позначення вибраної вершини як null
                selectedVertex = null;
                // Зменшення індексів вершин, які мають індекси більші за видалену вершину
                for (int i = removedIndex; i < vertices.Count; i++)
                {
                    vertices[i].VertexNumber -= 1;
                    vertices[i].Content = vertices[i].VertexNumber;
                }
            }
            // Оновлення вмісту мітки з вибраною вершиною
            label.Content = $"Selected Vertex: None\nPosition (X:{0} Y:{0})";
            // Оновлення матриці суміжності
            AdjacencyMatrix();
        }

        // Видалення ребра
        private void RemoveEdge(Edge edge)
        {
            // Видаляємо ребро з візуального контейнера
            graphCanvas.Children.Remove(edge);
            // Видаляємо ребро зі списку ребер
            edges.Remove(edge);
            // Оновлюємо позиції ребер, які виходять з початкової вершини
            UpdateEdgePositions(edge.StartVertex);
            // Оновлюємо матрицю суміжності
            AdjacencyMatrix();
        }

        // Видалення всіх ребер, що з'єднані з заданою вершиною
        private void RemoveEdgesConnectedToVertex(Vertex vertex)
        {
            // Цикл для проходження по усім ребрам у зворотному порядку
            for (int i = edges.Count - 1; i >= 0; i--)
            {
                // Отримуємо поточне ребро
                var edge = edges[i];

                // Перевіряємо, чи ребро з'єднане з початковим або кінцевим вершинами
                if (edge.StartVertex == vertex || edge.EndVertex == vertex)
                {
                    // Видаляємо ребро
                    RemoveEdge(edge);
                }
            }
        }

        // Обробка події включення кнопки створення ребра (активація кнопки Create Edge)
        private void ToggleEdgeCreationButton_Checked(object sender, RoutedEventArgs e)
        {
            isCreatingEdge = true;
        }

        // Обробка події вимкнення кнопки створення ребра (деактивація кнопки Create Edge)
        private void ToggleEdgeCreationButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (isCreatingEdge && edgeLine != null)
            {
                graphCanvas.Children.Remove(edgeLine);
                edgeLine = null;
            }
            isCreatingEdge = false;
        }
        // Метод створення матриці суміжності
        private void AdjacencyMatrix()
        {
            // Створення матриці суміжності
            int[,] adjacencyMatrix = new int[vertices.Count, vertices.Count];
            // Заповнення матриці суміжності з використанням індексів вершин у списку
            foreach (var edge in edges)
            {
                int startVertexIndex = vertices.IndexOf(edge.StartVertex);
                int endVertexIndex = vertices.IndexOf(edge.EndVertex);
                adjacencyMatrix[startVertexIndex, endVertexIndex] = 1;
            }
            // Очистити колекції стовпців та рядків у DataGrid
            matrixDataGrid.Columns.Clear();
            matrixDataGrid.Items.Clear();
            // Додати стовпець для номерів вершин
            matrixDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = " ",
                Binding = new Binding($"[{0}]")
            });
            // Додати стовпці для елементів матриці та заповнити дані
            for (int i = 0; i < vertices.Count; i++)
            {
                var column = new DataGridTextColumn
                {
                    Header = $"{vertices[i].VertexNumber}",
                    Binding = new Binding($"[{i + 1}]")
                };

                matrixDataGrid.Columns.Add(column);
            }
            // Заповнити рядки матриці
            for (int i = 0; i < vertices.Count; i++)
            {
                var item = new ObservableCollection<object>();
                item.Add(vertices[i].VertexNumber);
                for (int j = 0; j < vertices.Count; j++)
                {
                    item.Add(adjacencyMatrix[i, j]);
                }
                matrixDataGrid.Items.Add(item);
            }
        }
        // перевірка зміни розміра вікна
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var item in vertices)
            {
                UpdateEdgePositions(item);
            }
         
        }
    }
    public abstract class GraphElement : UserControl
    {
        public abstract void UpdatePosition();
    }
    public class Vertex : GraphElement
    {
        public int VertexNumber { get; set; }

        public double CenterX => Canvas.GetLeft(this) + (ActualWidth / 2);
        public double CenterY => Canvas.GetTop(this) + (ActualHeight / 2);

        UserControl a = new UserControl();
        public Vertex(int vertexNumber)
        {
            VertexNumber = vertexNumber;
            Content = vertexNumber.ToString();
            Width = 50;
            Height = 50;
            Background = Brushes.LightBlue;
            Style = CreateRoundButtonStyle();
            UpdatePosition();
        }

        private Style CreateRoundButtonStyle()
        {
            var style = new Style(typeof(Vertex));

            style.Setters.Add(new Setter(WidthProperty, 50.0));
            style.Setters.Add(new Setter(HeightProperty, 50.0));
            style.Setters.Add(new Setter(BackgroundProperty, Brushes.LightBlue));
            style.Setters.Add(new Setter(TemplateProperty, CreateControlTemplate()));

            return style;
        }

        private ControlTemplate CreateControlTemplate()
        {
            var template = new ControlTemplate(typeof(Vertex));
            var border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            border.SetValue(Border.BorderBrushProperty, Brushes.Black);
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(BackgroundProperty));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(25));
            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.ContentProperty, new TemplateBindingExtension(ContentProperty));

            border.AppendChild(contentPresenter);

            template.VisualTree = border;

            return template;
        }

        public override void UpdatePosition()
        {
            double newX = Canvas.GetLeft(this);
            double newY = Canvas.GetTop(this);

            Canvas.SetLeft(this, newX);
            Canvas.SetTop(this, newY);

            // Оновлення позиції ребер, пов'язаних з цією вершиною
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.UpdateEdgePositions(this);
        }
    }

    public class Edge : GraphElement
    {
        private Line line;
        private Arrowhead arrowhead;

        public Vertex StartVertex { get; }
        public Vertex EndVertex { get; }

        public Edge(Vertex startVertex, Vertex endVertex)
        {
            StartVertex = startVertex;
            EndVertex = endVertex;

            line = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            arrowhead = new Arrowhead
            {
                Fill = Brushes.Black
            };
            UpdatePosition();
            Panel.SetZIndex(this, -1);
            Content = new Canvas { Children = { line, arrowhead } };
            // Оновлення положення стрілки при зміні розташування вершин
            StartVertex.SizeChanged += Vertex_SizeChanged;
            EndVertex.SizeChanged += Vertex_SizeChanged;
        }

        public override void UpdatePosition()
        {
            double startX = StartVertex.CenterX;
            double startY = StartVertex.CenterY;
            double endX = EndVertex.CenterX;
            double endY = EndVertex.CenterY;

            double angle = Math.Atan2(endY - startY, endX - startX);

            double offset = StartVertex.ActualHeight / 2;

            double lineStartX = startX + Math.Cos(angle) * offset;
            double lineStartY = startY + Math.Sin(angle) * offset;
            double lineEndX = endX - Math.Cos(angle) * offset;
            double lineEndY = endY - Math.Sin(angle) * offset;

            line.X1 = lineStartX;
            line.Y1 = lineStartY;
            line.X2 = lineEndX;
            line.Y2 = lineEndY;

            double arrowheadSize = 10;
            double halfArrowheadSize = arrowheadSize / 2;
            double arrowheadOffset = StartVertex.ActualHeight / 2 + halfArrowheadSize;

            double arrowheadX = lineStartX + (lineEndX - lineStartX) / 1.02;
            double arrowheadY = lineStartY + (lineEndY - lineStartY) / 1.02;

            arrowhead.Width = arrowheadSize;
            arrowhead.Height = arrowheadSize;
            Canvas.SetLeft(arrowhead, arrowheadX - halfArrowheadSize);
            Canvas.SetTop(arrowhead, arrowheadY - halfArrowheadSize);
            arrowhead.RenderTransform = new RotateTransform(angle * (180 / Math.PI), halfArrowheadSize, halfArrowheadSize);

            Canvas.SetZIndex(this, Math.Min(Canvas.GetZIndex(StartVertex), Canvas.GetZIndex(EndVertex)) - 1);
        }

        private void Vertex_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePosition();
        }
    }

    public class Arrowhead : Shape
    {
        protected override Geometry DefiningGeometry
        {
            get
            {
                double arrowheadSize = 10;
                double halfArrowheadSize = arrowheadSize / 2;
                PathGeometry geometry = new PathGeometry();
                PathFigure figure = new PathFigure();
                figure.StartPoint = new Point(0, 0);
                figure.Segments.Add(new LineSegment(new Point(arrowheadSize, halfArrowheadSize), isStroked: true));
                figure.Segments.Add(new LineSegment(new Point(0, arrowheadSize), isStroked: true));
                figure.IsClosed = true;
                geometry.Figures.Add(figure);
                return geometry;
            }
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            var customPen = new Pen(Brushes.Black, 2.0);
            drawingContext.DrawGeometry(Fill, customPen, DefiningGeometry);
        }
    }

}
