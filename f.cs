private void ApplyTransformation()
        {
            for (int i = 0; i < currentShape.Length; i++)
            {
                // Преобразуем 2D-вектор в 4D-вектор, чтобы применить матрицу 4x4
                Vector4 vertex = new Vector4(currentShape[i], 0, 1);
                Vector4 transformedVertex = Vector4.Transform(vertex, translationMatrix);
                currentShape[i] = new Vector2(transformedVertex.X, transformedVertex.Y);
            }
        }
