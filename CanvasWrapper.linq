<Query Kind="Program">
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Drawing2D</Namespace>
</Query>

void Main()
{
	CanvasWrapperTest(ctx => CanvasWrapperTest_LineTo(ctx));
	CanvasWrapperTest(ctx => CanvasWrapperTest_ArcSmile(ctx));
	CanvasWrapperTest(ctx => CanvasWrapperTest_Arcs(ctx));
	CanvasWrapperTest(ctx => CanvasWrapperTest_Arcs2(ctx));
	CanvasWrapperTest(ctx => CanvasWrapperText_QuadricCurve(ctx));
	CanvasWrapperTest(ctx => CanvasWrapperTest_Heart(ctx));
	CanvasWrapperTest(ctx => CanvasWrapperTest_PacManSolo(ctx));
	CanvasWrapperTest(ctx => CanvasWrapperTest_PacMan(ctx));

	CanvasWrapperTest(ctx => CanvasWrapperFillTest(ctx), 200, 150);
	
	CanvasWrapperTest(ctx => CanvasWrapper_RectTest(ctx), 300, 150);
}

public class GraphicsCanvasWrapper : IDisposable
{
	public GraphicsCanvasWrapper(Graphics graphics)
	{
		g = graphics;
	}

	Graphics g = null;
	GraphicsPath path = new GraphicsPath();
	PointF lastPoint = PointF.Empty;

	Brush brush = new SolidBrush(Color.Black);
	Brush clearBrush = new SolidBrush(Color.Transparent);
	Pen pen = new Pen(Color.Black);

	public static PointF P(double x, double y)
	{
		return new PointF((float)x, (float)y);
	}
	public static PointF P(float x, float y)
	{
		return new PointF(x, y);
	}
	public static float ToDegrees(double radians)
	{
		return (float)(radians * 180 / Math.PI);
	}

	///Draws a filled rectangle
	public void fillRect(double x, double y, double width, double height)
	{
		//This draws instantly
		g.FillRectangle(brush, (float)x, (float)y, (float)width, (float)height);
	}
	///Draws a rectangular outline
	public void strokeRect(double x, double y, double width, double height)
	{
		//This draws instantly
		g.DrawRectangle(pen, (float)x, (float)y, (float)width, (float)height);
	}
	///Clears the specified area and makes it fully transparent
	public void clearRect(double x, double y, double width, double height)
	{
		//This draws instantly
		g.FillRectangle(clearBrush, (float)x, (float)y, (float)width, (float)height);
	}
	
	public void rect(double x, double y, double width, double height)
	{
		moveTo(x, y);
		lineTo(x, y+height);
		lineTo(x+width, y+height);
		lineTo(x+width, y);
		lineTo(x, y);
		closePath();
	}

	public void lineTo(double x, double y)
	{
		lineTo(P(x, y));
	}
	public void lineTo(PointF p)
	{
		path.AddLine(lastPoint, p);
		lastPoint = p;
	}
	public void closePath()
	{
		path.CloseFigure();
		lastPoint = PointF.Empty;
	}

	public void arc(double x, double y, double radius, double startAngle, double endAngle, bool anticlockwise)
	{
		float wh = (float)radius * 2;
		//		double start = startAngle;
		//		//TODO probably need to reduce end and start to under 2*PI or something...
		//		//double sweep = (anticlockwise ? startAngle + endAngle - 2*Math.PI : startAngle + endAngle);
		//		double sweep = startAngle + endAngle;
		//		//sweep.Dump();
		//		if (anticlockwise && sweep < (2*Math.PI)) sweep -= 2*Math.PI;

		//double start = (anticlockwise ? startAngle : endAngle);
		//double sweep = startAngle - endAngle;
		//if (anticlockwise) sweep += Math.PI;

		//		//THIS ONE HAS anticlockwise=false COMPLETELY CORRECT
		//		double start = (anticlockwise ? startAngle : endAngle);
		//		double end = (!anticlockwise ? startAngle : endAngle);
		//		//endAngle.Dump();
		//		double sweep = startAngle - endAngle;
		//		if (anticlockwise) sweep = (startAngle) + endAngle - 2*Math.PI;
		//		sweep.Dump();

		double start = 0;
		double sweep = 0;
		if (anticlockwise)
		{
			start = startAngle;
			sweep = endAngle - startAngle;
			while (sweep >= 0) sweep -= 2 * Math.PI;
			//sweep.Dump();
			//if (sweep <= 0) sweep += 2*Math.PI;
			//sweep.Dump();
		}
		else
		{
			if (endAngle < 0) endAngle += 2 * Math.PI;
			start = endAngle;
			sweep = startAngle - endAngle;
		}

		arc((float)(x - radius), (float)(y - radius), wh, wh, ToDegrees(start), ToDegrees(sweep));
		lastPoint = P(x + (Math.Sin(ToDegrees(endAngle)) * radius), y + (Math.Cos(ToDegrees(endAngle)) * radius));
		//	lastPoint.Dump();
	}
	private void arc(float x, float y, float width, float height, float startAngle, float sweepAngle)
	{
		path.AddArc(x, y, width, height, startAngle, sweepAngle);
		//lastPoint = P(x, y);
	}

	public void beginPath()
	{
		path.Dispose();
		path = new GraphicsPath();
		lastPoint = PointF.Empty;
	}
	public void moveTo(double x, double y)
	{
		lastPoint = P(x, y);
	}
	//	public void moveTo(float x, float y)
	//	{
	//		lastPoint = P(x, y);
	//	}

	public void quadraticCurveTo(double cp1x, double cp1y, double x, double y)
	{
		quadraticCurveTo(P(cp1x, cp1y), P(x, y));
	}
	public void quadraticCurveTo(PointF cp1, PointF pt2)
	{
		//path.AddCurve(new [] {lastPoint, cp1, pt2});
		//		var cpA = P((lastPoint.X + cp1.X + pt2.X) / 3, (lastPoint.Y + cp1.Y + pt2.Y) / 3);
		//		var cpA = P((lastPoint.X + cp1.X + cp1.X + pt2.X) / 4, (lastPoint.Y + cp1.Y + cp1.Y + pt2.Y) / 4);
		//		var cpA = P((lastPoint.X + cp1.X + cp1.X + cp1.X + pt2.X) / 5, (lastPoint.Y + cp1.Y + cp1.Y + cp1.Y + pt2.Y) / 5);
		//		path.AddCurve(new [] {lastPoint, cpA, pt2});
		path.AddBezier(lastPoint, cp1, cp1, pt2);
		lastPoint = pt2;
	}

	public void bezierCurveTo(double cp1x, double cp1y, double cp2x, double cp2y, double x, double y)
	{
		bezierCurveTo(P(cp1x, cp1y), P(cp2x, cp2y), P(x, y));
	}
	private void bezierCurveTo(PointF cp1, PointF cp2, PointF pt2)
	{
		path.AddBezier(lastPoint, cp1, cp2, pt2);
		lastPoint = pt2;
	}

	public string fillStyle
	{
		set
		{
			brush.Dispose();
			brush = new SolidBrush(Color.FromName(value));
		}
	}
	public Color fillStyleColor
	{
		set
		{
			brush.Dispose();
			brush = new SolidBrush(value);
		}
	}

	public string strokeStyle
	{
		set
		{
			//pen.Dispose();
			//pen = new Pen(Color.FromName(value));
			pen.Color = Color.FromName(value);
		}
	}
	public Color strokeStyleColor
	{
		set
		{
			//pen.Dispose();
			//pen = new Pen(value);
			pen.Color = value;
		}
	}
	
	public string lineWidth
	{
		set
		{
			var pixelWidth = int.Parse(value);
			
			pen.Width = pixelWidth;
		}
	}

	public void fill()
	{
		g.FillPath(brush, path);
	}
	public void stroke()
	{
		g.DrawPath(pen, path);
	}

	public void Dispose()
	{
		brush.Dispose();
		clearBrush.Dispose();
		pen.Dispose();
		path.Dispose();
	}
}
public void CanvasWrapperTest(Action<GraphicsCanvasWrapper> render, int width = 150, int height = 200)
{
	using (Bitmap b = new Bitmap(width, height))
	using (Graphics g = Graphics.FromImage(b))
	{
		g.SmoothingMode = SmoothingMode.HighQuality;

		using (GraphicsCanvasWrapper ctx = new GraphicsCanvasWrapper(g))
		{
			render(ctx);
		}

		b.Dump();
	}
}
public void CanvasWrapperTest_LineTo(GraphicsCanvasWrapper ctx)
{
	// Filled triangle
	ctx.beginPath();
	ctx.moveTo(25, 25);
	ctx.lineTo(105, 25);
	ctx.lineTo(25, 105);
	ctx.fill();

	// Stroked triangle
	ctx.beginPath();
	ctx.moveTo(125, 125);
	ctx.lineTo(125, 45);
	ctx.lineTo(45, 125);
	ctx.closePath();
	ctx.stroke();
}
public void CanvasWrapperTest_ArcSmile(GraphicsCanvasWrapper ctx)
{
	ctx.beginPath();
	ctx.arc(75, 75, 50, 0, Math.PI * 2, true); // Outer circle
	ctx.moveTo(110, 75);
	ctx.arc(75, 75, 35, 0, Math.PI, false);  // Mouth (clockwise)
	ctx.moveTo(65, 65);
	ctx.arc(60, 65, 5, 0, Math.PI * 2, true);  // Left eye
	ctx.moveTo(95, 65);
	ctx.arc(90, 65, 5, 0, Math.PI * 2, true);  // Right eye
	ctx.stroke();
}
public void CanvasWrapperTest_Arcs(GraphicsCanvasWrapper ctx)
{
	for (var i = 0; i < 4; i++)
	{
		for (var j = 0; j < 3; j++)
		{
			ctx.beginPath();
			var x = 25 + j * 50;               // x coordinate
			var y = 25 + i * 50;               // y coordinate
			var radius = 20;                    // Arc radius
			var startAngle = 0;                     // Starting point on circle
			var endAngle = Math.PI + (Math.PI * j) / 2; // End point on circle
			var anticlockwise = i % 2 == 0 ? false : true; // clockwise or anticlockwise

			ctx.arc(x, y, radius, startAngle, endAngle, anticlockwise);

			if (i > 1)
			{
				ctx.fill();
			}
			else
			{
				ctx.stroke();
			}
		}
	}
}
public void CanvasWrapperTest_Arcs2(GraphicsCanvasWrapper ctx)
{
	for (var i = 0; i < 4; i++)
	{
		for (var j = 0; j < 3; j++)
		{
			ctx.beginPath();
			var x = 25 + j * 50;               // x coordinate
			var y = 25 + i * 50;               // y coordinate
			var radius = 20;                    // Arc radius
			var startAngle = Math.PI / 7 * i;                     // Starting point on circle
			var endAngle = Math.PI / 7 * (2 + i + j); // End point on circle
			var anticlockwise = i % 2 == 0 ? false : true; // clockwise or anticlockwise

			ctx.arc(x, y, radius, startAngle, endAngle, anticlockwise);

			if (i > 1)
			{
				ctx.fill();
			}
			else
			{
				ctx.stroke();
			}
		}
	}
}
public void CanvasWrapperText_QuadricCurve(GraphicsCanvasWrapper ctx)
{
	ctx.beginPath();
	ctx.moveTo(75, 25);
	ctx.quadraticCurveTo(25, 25, 25, 62.5);
	ctx.quadraticCurveTo(25, 100, 50, 100);
	ctx.quadraticCurveTo(50, 120, 30, 125);
	ctx.quadraticCurveTo(60, 120, 65, 100);
	ctx.quadraticCurveTo(125, 100, 125, 62.5);
	ctx.quadraticCurveTo(125, 25, 75, 25);
	ctx.stroke();
}
public void CanvasWrapperTest_Heart(GraphicsCanvasWrapper ctx)
{
	ctx.beginPath();
	ctx.moveTo(75, 40);
	ctx.bezierCurveTo(75, 37, 70, 25, 50, 25);
	ctx.bezierCurveTo(20, 25, 20, 62.5, 20, 62.5);
	ctx.bezierCurveTo(20, 80, 40, 102, 75, 120);
	ctx.bezierCurveTo(110, 102, 130, 80, 130, 62.5);
	ctx.bezierCurveTo(130, 62.5, 130, 25, 100, 25);
	ctx.bezierCurveTo(85, 25, 75, 37, 75, 40);
	ctx.fill();
}
public void CanvasWrapperTest_PacManSolo(GraphicsCanvasWrapper ctx)
{
	ctx.beginPath();
	ctx.arc(37, 37, 13, Math.PI / 7, -Math.PI / 7, false);
	ctx.lineTo(31, 37);
	ctx.fill();
}
public void CanvasWrapperTest_PacMan(GraphicsCanvasWrapper ctx)
{
	roundedRect(ctx, 12, 12, 150, 150, 15);
	roundedRect(ctx, 19, 19, 150, 150, 9);
	roundedRect(ctx, 53, 53, 49, 33, 10);
	roundedRect(ctx, 53, 119, 49, 16, 6);
	roundedRect(ctx, 135, 53, 49, 33, 10);
	roundedRect(ctx, 135, 119, 25, 49, 10);

	ctx.beginPath();
	ctx.arc(37, 37, 13, Math.PI / 7, -Math.PI / 7, false);
	ctx.lineTo(31, 37);
	ctx.fill();
	for (var i = 0; i < 8; i++)
	{
		ctx.fillRect(51 + i * 16, 35, 4, 4);
	}
	for (var i = 0; i < 6; i++)
	{
		ctx.fillRect(115, 51 + i * 16, 4, 4);
	}
	for (var i = 0; i < 8; i++)
	{
		ctx.fillRect(51 + i * 16, 99, 4, 4);
	}
	ctx.beginPath();
	ctx.moveTo(83, 116);
	ctx.lineTo(83, 102);
	ctx.bezierCurveTo(83, 94, 89, 88, 97, 88);
	ctx.bezierCurveTo(105, 88, 111, 94, 111, 102);
	ctx.lineTo(111, 116);
	ctx.lineTo(106.333, 111.333);
	ctx.lineTo(101.666, 116);
	ctx.lineTo(97, 111.333);
	ctx.lineTo(92.333, 116);
	ctx.lineTo(87.666, 111.333);
	ctx.lineTo(83, 116);
	ctx.fill();
	ctx.fillStyle = "white";
	ctx.beginPath();
	ctx.moveTo(91, 96);
	ctx.bezierCurveTo(88, 96, 87, 99, 87, 101);
	ctx.bezierCurveTo(87, 103, 88, 106, 91, 106);
	ctx.bezierCurveTo(94, 106, 95, 103, 95, 101);
	ctx.bezierCurveTo(95, 99, 94, 96, 91, 96);
	ctx.moveTo(103, 96);
	ctx.bezierCurveTo(100, 96, 99, 99, 99, 101);
	ctx.bezierCurveTo(99, 103, 100, 106, 103, 106);
	ctx.bezierCurveTo(106, 106, 107, 103, 107, 101);
	ctx.bezierCurveTo(107, 99, 106, 96, 103, 96);
	ctx.fill();
	ctx.fillStyle = "black";
	ctx.beginPath();
	ctx.arc(101, 102, 2, 0, Math.PI * 2, true);
	ctx.fill();
	ctx.beginPath();
	ctx.arc(89, 102, 2, 0, Math.PI * 2, true);
	ctx.fill();
}
public void roundedRect(GraphicsCanvasWrapper ctx, double x, double y, double width, double height, double radius)
{
	ctx.beginPath();
	ctx.moveTo(x, y + radius);
	ctx.lineTo(x, y + height - radius);
	ctx.quadraticCurveTo(x, y + height, x + radius, y + height);
	ctx.lineTo(x + width - radius, y + height);
	ctx.quadraticCurveTo(x + width, y + height, x + width, y + height - radius);
	ctx.lineTo(x + width, y + radius);
	ctx.quadraticCurveTo(x + width, y, x + width - radius, y);
	ctx.lineTo(x + radius, y);
	ctx.quadraticCurveTo(x, y, x, y + radius);
	ctx.stroke();
}


void CanvasWrapperFillTest(GraphicsCanvasWrapper ctx)
{
	ctx.beginPath();
	ctx.rect(20, 20, 150, 100);
	ctx.fillStyle = "red";
	ctx.fill();

	ctx.beginPath();
	ctx.rect(40, 40, 150, 100);
	ctx.fillStyle = "blue";
	ctx.fill();
}
void CanvasWrapper_RectTest(GraphicsCanvasWrapper ctx)
{
	//https://www.w3schools.com/tags/canvas_rect.asp
	
	
	// Red rectangle
	ctx.beginPath();
	ctx.lineWidth = "6";
	ctx.strokeStyle = "red";
	ctx.rect(5, 5, 290, 140);
	ctx.stroke();

	// Green rectangle
	ctx.beginPath();
	ctx.lineWidth = "4";
	ctx.strokeStyle = "green";
	ctx.rect(30, 30, 50, 50);
	ctx.stroke();

	// Blue rectangle
	ctx.beginPath();
	ctx.lineWidth = "10";
	ctx.strokeStyle = "blue";
	ctx.rect(50, 50, 150, 80);
	ctx.stroke();
}
