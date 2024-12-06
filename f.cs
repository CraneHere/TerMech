protected override void OnUpdateFrame(FrameEventArgs args)
        {
            // Обновляем время для анимации
            elapsedTime += (float)args.Time;
            moveSpeed = 50.0f * elapsedTime; // Перемещение вдоль оси X на 50 пикселей в секунду

            ApplyTransformation();

            // Обновляем буфер вершин с новыми координатами
            VertexPositionColor[] vertices = new VertexPositionColor[this.currentShape.Length];
            for (int i = 0; i < this.currentShape.Length; i++)
            {
                vertices[i] = new VertexPositionColor(this.currentShape[i], new Color4(0.2f, 0.4f, 0.8f, 1f));
            }

            this.vertexBuffer.SetData(vertices, vertices.Length);

            base.OnUpdateFrame(args);
        }

        private void ApplyTransformation()
        {
            // Создаем матрицу трансляции, перемещающую шестиугольник
            translationMatrix = Matrix4.CreateTranslation(moveSpeed, 0, 0); // Перемещение вдоль оси X

            for (int i = 0; i < currentShape.Length; i++)
            {
                // Преобразуем 2D-вектор в 4D-вектор, чтобы применить матрицу 4x4
                Vector4 vertex = new Vector4(currentShape[i].X, currentShape[i].Y, 0, 1);
                Vector4 transformedVertex = translationMatrix * vertex; // Применяем преобразование
                currentShape[i] = new Vector2(transformedVertex.X, transformedVertex.Y);
            }
        }
