 protected override void OnUpdateFrame(FrameEventArgs args)
        {
            // Обновляем координаты текущих вершин, чтобы перемещать шестиугольник
            for (int i = 0; i < currentShape.Length; i++)
            {
                currentShape[i] += movementDirection; // Добавляем смещение к каждой вершине
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
