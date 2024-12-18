using OpenTK.Graphics.OpenGL4;
using System;

namespace CompGraph
{
    public class VertexArray : IDisposable
    {
        public int VertexArrayHandle { get; private set; }

        private bool disposed = false;

        // Конструктор
        public VertexArray(VertexBuffer vertexBuffer)
        {
            // Создаем объект Vertex Array
            VertexArrayHandle = GL.GenVertexArray();
            
            // Привязываем Vertex Array
            GL.BindVertexArray(VertexArrayHandle);

            // Привязываем Vertex Buffer к текущему контексту
            vertexBuffer.Bind();

            // Развязываем Vertex Array (мы привязали его, теперь можем работать с буферами)
            GL.BindVertexArray(0);
        }

        // Освобождаем ресурсы
        public void Dispose()
        {
            if (disposed)
                return;

            GL.DeleteVertexArray(VertexArrayHandle);

            disposed = true;
            GC.SuppressFinalize(this);
        }

        // Метод для привязки этого объекта VertexArray
        public void Bind()
        {
            GL.BindVertexArray(VertexArrayHandle);
        }

        // Метод для отвязки этого объекта VertexArray
        public void Unbind()
        {
            GL.BindVertexArray(0);
        }
    }
}
