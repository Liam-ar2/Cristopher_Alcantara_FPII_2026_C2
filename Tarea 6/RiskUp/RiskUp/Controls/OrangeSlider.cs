namespace RiskUp.Controls;

public class OrangeSlider : Control
{
    private int _minimum = 1;
    private int _maximum = 5;
    private int _value = 1;
    private bool _arrastrando = false;

    private static readonly Color ColorPista = Color.FromArgb(90, 90, 80);
    private static readonly Color ColorAcento = Color.FromArgb(255, 159, 28);
    private static readonly Color ColorFondo = Color.Transparent;

    public event EventHandler? ValueChanged;

    public int Minimum
    {
        get => _minimum;
        set { _minimum = value; Invalidate(); }
    }

    public int Maximum
    {
        get => _maximum;
        set { _maximum = value; Invalidate(); }
    }

    public int Value
    {
        get => _value;
        set
        {
            int nuevo = Math.Clamp(value, _minimum, _maximum);
            if (nuevo != _value)
            {
                _value = nuevo;
                Invalidate();
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public OrangeSlider()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);

        BackColor = Color.Transparent;
        Height = 28;
        Cursor = Cursors.Hand;
    }

    private const int ThumbRadius = 10;
    private int TrackLeft => ThumbRadius;
    private int TrackRight => Width - ThumbRadius;
    private int TrackY => Height / 2;

    private int ValueToX(int value)
    {
        if (_maximum == _minimum) return TrackLeft;
        double ratio = (double)(value - _minimum) / (_maximum - _minimum);
        return TrackLeft + (int)(ratio * (TrackRight - TrackLeft));
    }

    private int XToValue(int x)
    {
        if (TrackRight == TrackLeft) return _minimum;
        double ratio = (double)(x - TrackLeft) / (TrackRight - TrackLeft);
        ratio = Math.Clamp(ratio, 0, 1);
        return _minimum + (int)Math.Round(ratio * (_maximum - _minimum));
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        _arrastrando = true;
        Value = XToValue(e.X);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_arrastrando)
        {
            Value = XToValue(e.X);
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _arrastrando = false;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        int thumbX = ValueToX(_value);

        
        using (var penPista = new Pen(ColorPista, 4) { StartCap = System.Drawing.Drawing2D.LineCap.Round, EndCap = System.Drawing.Drawing2D.LineCap.Round })
        {
            g.DrawLine(penPista, TrackLeft, TrackY, TrackRight, TrackY);
        }

        
        using (var penActivo = new Pen(ColorAcento, 4) { StartCap = System.Drawing.Drawing2D.LineCap.Round, EndCap = System.Drawing.Drawing2D.LineCap.Round })
        {
            g.DrawLine(penActivo, TrackLeft, TrackY, thumbX, TrackY);
        }

       
        using (var brochaThumb = new SolidBrush(ColorAcento))
        {
            g.FillEllipse(brochaThumb, thumbX - ThumbRadius, TrackY - ThumbRadius, ThumbRadius * 2, ThumbRadius * 2);
        }
    }
}
