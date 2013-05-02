using System;
using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circular.Display {
    public class LineBatch {
        private const int DefaultBufferSize = 500;

        // a basic effect, which contains the shaders that we will use to draw our
        // primitives.
        private BasicEffect _basicEffect;

        private GraphicsDevice _device;

        private readonly Texture2D blank;


        public LineBatch ( GraphicsDevice graphicsDevice ) {
            if ( graphicsDevice == null ) {
                throw new ArgumentNullException( "graphicsDevice" );
            }

            blank = new Texture2D( graphicsDevice, 1, 1, false, SurfaceFormat.Color );
            blank.SetData( new[] { Color.White } );
        }



        public void DrawLine ( SpriteBatch batch, float width, Color color, Vector2 point1, Vector2 point2 ) {
            point1 = ConvertUnits.ToDisplayUnits( point1 );
            point2 = ConvertUnits.ToDisplayUnits( point2 );
            float angle = (float) Math.Atan2( point2.Y - point1.Y, point2.X - point1.X );
            float length = Vector2.Distance( point1, point2 );

            batch.Draw( blank, point1, null, color,
                       angle, Vector2.Zero, new Vector2( length, width ),
                       SpriteEffects.None, 0 );
        }

        public void DrawLineShape ( SpriteBatch batch, Shape shape, Color color, float width ) {
            if ( shape.ShapeType != ShapeType.Edge &&
                shape.ShapeType != ShapeType.Chain ) {
                throw new NotSupportedException( "The specified shapeType is not supported by LineBatch." );
            }
            switch ( shape.ShapeType ) {
                case ShapeType.Edge: {
                        EdgeShape edge = (EdgeShape) shape;
                        DrawLine( batch, width, color, edge.Vertex1, edge.Vertex2 );
                    }
                    break;
                case ShapeType.Chain: {
                        ChainShape chain = (ChainShape) shape;
                        for ( int i = 0; i < chain.Vertices.Count; ++i ) {
                            DrawLine( batch, width, color, chain.Vertices[ i ], chain.Vertices.NextVertex( i ) );
                        }
                    }
                    break;
            }
        }
    }
}