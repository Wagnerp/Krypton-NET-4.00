﻿// *****************************************************************************
// BSD 3-Clause License (https://github.com/ComponentFactory/Krypton/blob/master/LICENSE)
//  © Component Factory Pty Ltd, 2006-2019, All rights reserved.
// The software and associated documentation supplied hereunder are the 
//  proprietary information of Component Factory Pty Ltd, 13 Swallows Close, 
//  Mornington, Vic 3931, Australia and are supplied subject to license terms.
// 
//  Modifications by Peter Wagner(aka Wagnerp) & Simon Coghlan(aka Smurf-IV) 2017 - 2019. All rights reserved. (https://github.com/Wagnerp/Krypton-NET-5.470)
//  Version 5.470.0.0  www.ComponentFactory.com
// *****************************************************************************

using System.Drawing;
using System.Collections.Generic;

namespace ComponentFactory.Krypton.Ribbon
{
    internal class KeyTipInfoList : List<KeyTipInfo> {};

    internal class KeyTipInfo
    {
        #region Instance Fields

        private readonly IRibbonKeyTipTarget _target;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the KeyTipInfo class.
        /// </summary>
        /// <param name="enabled">Enabled state of the item.</param>
        /// <param name="keyString">String of characters used to activate item.</param>
        /// <param name="screenPt">Screen coordinate for center of keytip.</param>
        /// <param name="clientRect">Client rectangle for keytip.</param>
        /// <param name="target">Target to invoke when item is selected.</param>
        public KeyTipInfo(bool enabled,
                          string keyString,
                          Point screenPt,
                          Rectangle clientRect,
                          IRibbonKeyTipTarget target)
        {
            Enabled = enabled;
            KeyString = keyString;
            ScreenPt = screenPt;
            ClientRect = clientRect;
            _target = target;
            Visible = true;
        }
        #endregion

        #region Public
        /// <summary>
        /// Gets the enabled state of the source item.
        /// </summary>
        public bool Enabled { get; }

        /// <summary>
        /// Gets and sets the visible state of the key tip.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets the string used to select the item.
        /// </summary>
        public string KeyString { get; }

        /// <summary>
        /// Gets the center screen location for showing the keytip.
        /// </summary>
        public Point ScreenPt { get; }

        /// <summary>
        /// Gets the client rectangle for showing the keytip.
        /// </summary>
        public Rectangle ClientRect { get; }

        /// <summary>
        /// Perform actual selection of the item.
        /// </summary>
        /// <param name="ribbon">Reference to owning ribbon instance.</param>
        public void KeyTipSelect(KryptonRibbon ribbon)
        {
            _target?.KeyTipSelect(ribbon);
        }
        #endregion
    }
}
