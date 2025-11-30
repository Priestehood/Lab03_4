using System;
using System.Windows.Forms;

namespace Lab04_4
{
    /// <summary>
    /// 地图操作枚举类型
    /// </summary>
    public enum MapOperationType
    {
        /// <summary>
        /// 默认（无操作）
        /// </summary>
        Default,

        /// <summary>
        /// 从地图上创建要素
        /// </summary>
        CreateFeature,

        /// <summary>
        /// 编辑要素
        /// </summary>
        EditFeature,

        /// <summary>
        /// 删除要素
        /// </summary>
        DeleteFeature,

        /// <summary>
        /// 选择要素
        /// </summary>
        SelectFeature,

        /// <summary>
        /// 标识/显示要素信息
        /// </summary>
        IdentifyFeature,

        /// <summary>
        /// 点击查询建筑/道路
        /// </summary>
        ElementQuery,

        /// <summary>
        /// 绘制多义线
        /// </summary>
        DrawPolyline,

        /// <summary>
        /// 高程插值
        /// </summary>
        IntepolateElevation,
    }

    public partial class Form_4 : Form
    {
        #region 私有变量
        private MapOperationType mapOperation = MapOperationType.Default;
        #endregion
    }
}