protected override void OnUpdateFrame(FrameEventArgs args)
        {
            // Создаем матрицу трансляции, которая будет перемещать шестиугольник
            translationMatrix = Matrix4.CreateTranslation(moveSpeed, 0, 0); // Перемещение вдоль оси X

            // Применяем матрицу к каждой вершине
            for (int i = 0; i < currentShape.Length; i++)
            {
                Vector4 vertex = new Vector4(currentShape[i].X, currentShape[i].Y, 0, 1);
                Vector4 transformedVertex = translationMatrix * vertex; // Применяем матрицу трансляции
                currentShape[i] = new Vector2(transformedVertex.X, transformedVertex.Y);
            }

            // Обновляем буфер вершин с новыми координатами
            VertexPositionColor[] vertices = new VertexPositionColor[this.currentShape.Length];
            for (int i = 0; i < this.currentShape.Length; i++)
            {
                vertices[i] = new VertexPositionColor(this.currentShape[i], new Color4(0.2f, 0.4f, 0.8f, 1f));
            }

            this.vertexBuffer.SetData(vertices, vertices.Length);

            base.OnUpdateFrame(args);
        }
