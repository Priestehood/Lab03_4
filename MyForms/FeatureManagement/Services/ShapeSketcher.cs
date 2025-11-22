using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using System;

namespace Lab03_4.MyForms.FeatureManagement.Services
{
    /// <summary>
    /// 绘制图形形状枚举类型
    /// </summary>
    public enum Shape
    {
        /// <summary>
        /// 默认（无操作）
        /// </summary>
        Default,

        /// <summary>
        /// 画点
        /// </summary>
        Point,
        /// <summary>
        /// 画线
        /// </summary>
        Polyline,
        /// <summary>
        /// 画多边形
        /// </summary>
        Polygon,
        /// <summary>
        /// 画矩形
        /// </summary>
        Rectangle
    }

    public class ShapeSketcher
    {
        public Shape shape = Shape.Default;

        IRubberBand rubber;

        public IGeometry Sketch(AxMapControl map, IMapControlEvents2_OnMouseUpEvent e)
        {
            if (e.button == 2)
            {   // 右键点击
                return null;
            }
            else
            if (e.button != 1)
            {   // 非左键点击
                return null;
            }
            // 左键点击

            IPoint point = CreatePoint(e);
            switch (shape)
            {
                case Shape.Point:
                    return point;
                case Shape.Polygon:
                    rubber = new RubberPolygon();
                    return rubber.TrackNew(map.ActiveView.ScreenDisplay, null);
                case Shape.Polyline:
                    rubber = new RubberLine();
                    return rubber.TrackNew(map.ActiveView.ScreenDisplay, null);
                case Shape.Rectangle:
                    rubber = new RubberRectangularPolygon();
                    return rubber.TrackNew(map.ActiveView.ScreenDisplay, null);
                default:
                    return null;
            }
        }
        private IPoint CreatePoint(IMapControlEvents2_OnMouseUpEvent e)
        {
            IPoint point = new Point();
            point.X = e.mapX;
            point.Y = e.mapY;
            return point;
        }
    }
}
