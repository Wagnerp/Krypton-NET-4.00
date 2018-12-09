﻿// *****************************************************************************
// BSD 3-Clause License (https://github.com/ComponentFactory/Krypton/blob/master/LICENSE)
//  © Component Factory Pty Ltd, 2006-2018, All rights reserved.
// The software and associated documentation supplied hereunder are the 
//  proprietary information of Component Factory Pty Ltd, 13 Swallows Close, 
//  Mornington, Vic 3931, Australia and are supplied subject to licence terms.
// 
//  Modifications by Peter Wagner(aka Wagnerp) & Simon Coghlan(aka Smurf-IV) 2017 - 2018. All rights reserved. (https://github.com/Wagnerp/Krypton-NET-5.4000)
//  Version 5.4000.0.0  www.ComponentFactory.com
// *****************************************************************************

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using ComponentFactory.Krypton.Toolkit;

namespace ComponentFactory.Krypton.Ribbon
{
	/// <summary>
	/// Draws a ribbon group color button.
	/// </summary>
    internal class ViewDrawRibbonGroupColorButton : ViewComposite,
                                                    IRibbonViewGroupItemView
    {
        #region Static Fields
        private static readonly Padding _largeImagePadding = new Padding(3, 2, 3, 3);
        private static readonly Padding _smallImagePadding = new Padding(3);
        #endregion

        #region Instance Fields
        private readonly KryptonRibbon _ribbon;
        private readonly NeedPaintHandler _needPaint;
        private ViewDrawRibbonGroupButtonBackBorder _viewLarge;
        private ViewLayoutRibbonRowCenter _viewLargeCenter;
        private ViewDrawRibbonGroupColorButtonImage _viewLargeImage;
        private ViewDrawRibbonGroupColorButtonText _viewLargeText1;
        private ViewDrawRibbonGroupColorButtonText _viewLargeText2;
        private ViewDrawRibbonDropArrow _viewLargeDropArrow;
        private ViewLayoutRibbonSeparator _viewLargeText2Sep1;
        private ViewLayoutRibbonSeparator _viewLargeText2Sep2;
        private ViewDrawRibbonGroupButtonBackBorder _viewMediumSmall;
        private ViewLayoutRibbonRowCenter _viewMediumSmallCenter;
        private ViewDrawRibbonGroupColorButtonImage _viewMediumSmallImage;
        private ViewDrawRibbonGroupColorButtonText _viewMediumSmallText1;
        private ViewDrawRibbonGroupColorButtonText _viewMediumSmallText2;
        private ViewDrawRibbonDropArrow _viewMediumSmallDropArrow;
        private ViewLayoutRibbonSeparator _viewMediumSmallText2Sep2;
        private ViewLayoutRibbonSeparator _viewMediumSmallText2Sep3;
        private GroupItemSize _currentSize;
        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the ViewDrawRibbonGroupColorButton class.
		/// </summary>
        /// <param name="ribbon">Reference to owning ribbon control.</param>
        /// <param name="ribbonColorButton">Reference to source color button definition.</param>
        /// <param name="needPaint">Delegate for notifying paint requests.</param>
        public ViewDrawRibbonGroupColorButton(KryptonRibbon ribbon,
                                              KryptonRibbonGroupColorButton ribbonColorButton,
                                              NeedPaintHandler needPaint)
        {
            Debug.Assert(ribbon != null);
            Debug.Assert(ribbonColorButton != null);
            Debug.Assert(needPaint != null);

            // Remember incoming references
            _ribbon = ribbon;
            GroupColorButton = ribbonColorButton;
            _needPaint = needPaint;
            _currentSize = GroupColorButton.ItemSizeCurrent;

            // Associate this view with the source component (required for design time selection)
            Component = GroupColorButton;

            // Create the different views for different sizes of the button
            CreateLargeButtonView();
            CreateMediumSmallButtonView();

            // Update all views to reflect current button state
            UpdateEnabledState();
            UpdateCheckedState();
            UpdateDropDownState();
            UpdateItemSizeState();

            // Hook into changes in the ribbon button definition
            GroupColorButton.PropertyChanged += OnButtonPropertyChanged;
        }

		/// <summary>
		/// Obtains the String representation of this instance.
		/// </summary>
		/// <returns>User readable name of the instance.</returns>
		public override string ToString()
		{
			// Return the class name and instance identifier
            return "ViewDrawRibbonGroupColorButton:" + Id;
		}

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (GroupColorButton != null)
                {
                    // Must unhook to prevent memory leaks
                    GroupColorButton.PropertyChanged -= OnButtonPropertyChanged;

                    // Remove association with definition
                    GroupColorButton.ColorButtonView = null;
                    GroupColorButton = null;
                }
            }

            base.Dispose(disposing);
        }
        #endregion

        #region GroupButton
        /// <summary>
        /// Gets access to the connected color button definition.
        /// </summary>
        public KryptonRibbonGroupColorButton GroupColorButton { get; private set; }

        #endregion

        #region GetFirstFocusItem
        /// <summary>
        /// Gets the first focus item from the item.
        /// </summary>
        /// <returns>ViewBase of item; otherwise false.</returns>
        public ViewBase GetFirstFocusItem()
        {
            // Only take focus if we are visible and enabled
            if (GroupColorButton.Visible && GroupColorButton.Enabled)
            {
                return _viewLarge == GroupColorButton.ColorButtonView ? _viewLarge : _viewMediumSmall;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region GetLastFocusItem
        /// <summary>
        /// Gets the last focus item from the item.
        /// </summary>
        /// <returns>ViewBase of item; otherwise false.</returns>
        public ViewBase GetLastFocusItem()
        {
            // Only take focus if we are visible and enabled
            if (GroupColorButton.Visible && GroupColorButton.Enabled)
            {
                return _viewLarge == GroupColorButton.ColorButtonView ? _viewLarge : _viewMediumSmall;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region GetNextFocusItem
        /// <summary>
        /// Gets the next focus item based on the current item as provided.
        /// </summary>
        /// <param name="current">The view that is currently focused.</param>
        /// <param name="matched">Has the current focus item been matched yet.</param>
        /// <returns>ViewBase of item; otherwise false.</returns>
        public ViewBase GetNextFocusItem(ViewBase current, ref bool matched)
        {
            // Do we match the current item?
            matched = (current == _viewLarge) || (current == _viewMediumSmall);
            return null;
        }
        #endregion

        #region GetPreviousFocusItem
        /// <summary>
        /// Gets the previous focus item based on the current item as provided.
        /// </summary>
        /// <param name="current">The view that is currently focused.</param>
        /// <param name="matched">Has the current focus item been matched yet.</param>
        /// <returns>ViewBase of item; otherwise false.</returns>
        public ViewBase GetPreviousFocusItem(ViewBase current, ref bool matched)
        {
            // Do we match the current item?
            matched = (current == _viewLarge) || (current == _viewMediumSmall);
            return null;
        }
        #endregion

        #region GetGroupKeyTips
        /// <summary>
        /// Gets the array of group level key tips.
        /// </summary>
        /// <param name="keyTipList">List to add new entries into.</param>
        /// <param name="lineHint">Provide hint to item about its location.</param>
        public void GetGroupKeyTips(KeyTipInfoList keyTipList, int lineHint)
        {
            // Only provide a key tip if we are visible
            if (Visible)
            {
                // Get the screen location of the button
                Rectangle viewRect = _ribbon.KeyTipToScreen(this[0]);

                Point screenPt = Point.Empty;
                GroupButtonController controller = null;

                // Determine the screen position of the key tip dependant on item location/size
                switch (_currentSize)
                {
                    case GroupItemSize.Large:
                        screenPt = new Point(viewRect.Left + (viewRect.Width / 2), viewRect.Bottom);
                        controller = _viewLarge.Controller;
                        break;
                    case GroupItemSize.Medium:
                    case GroupItemSize.Small:
                        screenPt = _ribbon.CalculatedValues.KeyTipRectToPoint(viewRect, lineHint);
                        controller = _viewMediumSmall.Controller;
                        break;
                }

                keyTipList.Add(new KeyTipInfo(GroupColorButton.Enabled, GroupColorButton.KeyTip, 
                                              screenPt, this[0].ClientRectangle, controller));
            }
        }
        #endregion

        #region Layout
        /// <summary>
        /// Override the group item size if possible.
        /// </summary>
        /// <param name="size">New size to use.</param>
        public void SetGroupItemSize(GroupItemSize size)
        {
            UpdateItemSizeState(size);
        }

        /// <summary>
        /// Reset the group item size to the item definition.
        /// </summary>
        public void ResetGroupItemSize()
        {
            UpdateItemSizeState();
        }

        /// <summary>
        /// Discover the preferred size of the element.
        /// </summary>
        /// <param name="context">Layout context.</param>
        public override Size GetPreferredSize(ViewLayoutContext context)
        {
            bool drawNonTrackingAreas = (_ribbon.RibbonShape != PaletteRibbonShape.Office2010 || _ribbon.RibbonShape == PaletteRibbonShape.Office2013);

            // Update the views with the type of button being used
            _viewLarge.ButtonType = GroupColorButton.ButtonType;
            _viewLarge.DrawNonTrackingAreas = drawNonTrackingAreas;
            _viewMediumSmall.ButtonType = GroupColorButton.ButtonType;
            _viewMediumSmall.DrawNonTrackingAreas = drawNonTrackingAreas;

            // Get the preferred size of button view
            Size preferredSize = base.GetPreferredSize(context);

            preferredSize.Height = _currentSize == GroupItemSize.Large
                ? _ribbon.CalculatedValues.GroupTripleHeight
                : _ribbon.CalculatedValues.GroupLineHeight;

            return preferredSize;
        }

        /// <summary>
        /// Perform a layout of the elements.
        /// </summary>
        /// <param name="context">Layout context.</param>
        public override void Layout(ViewLayoutContext context)
        {
            Debug.Assert(context != null);

            // Update our enabled and checked state
            UpdateEnabledState();
            UpdateCheckedState();
            UpdateDropDownState();

            // We take on all the available display area
            ClientRectangle = context.DisplayRectangle;

            // Let child elements layout in given space
            base.Layout(context);

            // For split buttons we need to calculate the split button areas
            if (GroupColorButton.ButtonType == GroupButtonType.Split)
            {
                // Find the start positions of the split areas for both views
                int largeSplitTop = _viewLargeImage.ClientRectangle.Bottom + 2;
                int mediumSmallSplitRight = _viewMediumSmallText2Sep2.ClientLocation.X;

                // Update the background/border view so it can draw appropriately
                _viewLarge.SplitRectangle = new Rectangle(ClientLocation.X, largeSplitTop, ClientWidth, ClientRectangle.Bottom - largeSplitTop);
                _viewMediumSmall.SplitRectangle = new Rectangle(mediumSmallSplitRight, ClientLocation.Y, ClientRectangle.Right - mediumSmallSplitRight, ClientHeight);
            }
            else
            {
                _viewLarge.SplitRectangle = Rectangle.Empty;
                _viewMediumSmall.SplitRectangle = Rectangle.Empty; 
            }
        }
        #endregion

        #region Protected
        /// <summary>
        /// Raises the NeedPaint event.
        /// </summary>
        /// <param name="needLayout">Does the palette change require a layout.</param>
        protected virtual void OnNeedPaint(bool needLayout)
        {
            OnNeedPaint(needLayout, Rectangle.Empty);
        }

        /// <summary>
        /// Raises the NeedPaint event.
        /// </summary>
        /// <param name="needLayout">Does the palette change require a layout.</param>
        /// <param name="invalidRect">Rectangle to invalidate.</param>
        protected virtual void OnNeedPaint(bool needLayout, Rectangle invalidRect)
        {
            if (_needPaint != null)
            {
                _needPaint(this, new NeedLayoutEventArgs(needLayout, invalidRect));

                if (needLayout)
                {
                    _ribbon.PerformLayout();
                }
            }
        }
        #endregion

        #region Implementation
        private void CreateLargeButtonView()
        {
            // Create the background and border view
            _viewLarge = new ViewDrawRibbonGroupButtonBackBorder(_ribbon, GroupColorButton,
                                                                 _ribbon.StateCommon.RibbonGroupButton.PaletteBack,
                                                                 _ribbon.StateCommon.RibbonGroupButton.PaletteBorder,
                                                                 false, _needPaint)
            {
                SplitVertical = true
            };
            _viewLarge.Click += OnLargeButtonClick;
            _viewLarge.DropDown += OnLargeButtonDropDown;

            if (_ribbon.InDesignMode)
            {
                _viewLarge.ContextClick += OnContextClick;
            }

            // Create the layout docker for the contents of the button
            ViewLayoutDocker contentLayout = new ViewLayoutDocker();

            // Add the large button at the top
            _viewLargeImage = new ViewDrawRibbonGroupColorButtonImage(_ribbon, GroupColorButton, true);
            ViewLayoutRibbonCenterPadding largeImagePadding = new ViewLayoutRibbonCenterPadding(_largeImagePadding)
            {
                _viewLargeImage
            };
            contentLayout.Add(largeImagePadding, ViewDockStyle.Top);

            // Add the first line of text
            _viewLargeText1 = new ViewDrawRibbonGroupColorButtonText(_ribbon, GroupColorButton, true);
            contentLayout.Add(_viewLargeText1, ViewDockStyle.Bottom);

            // Add the second line of text
            _viewLargeCenter = new ViewLayoutRibbonRowCenter();
            _viewLargeText2 = new ViewDrawRibbonGroupColorButtonText(_ribbon, GroupColorButton, false);
            _viewLargeDropArrow = new ViewDrawRibbonDropArrow(_ribbon);
            _viewLargeText2Sep1 = new ViewLayoutRibbonSeparator(4, false);
            _viewLargeText2Sep2 = new ViewLayoutRibbonSeparator(4, false);
            _viewLargeCenter.Add(_viewLargeText2);
            _viewLargeCenter.Add(_viewLargeText2Sep1);
            _viewLargeCenter.Add(_viewLargeDropArrow);
            _viewLargeCenter.Add(_viewLargeText2Sep2);
            contentLayout.Add(_viewLargeCenter, ViewDockStyle.Bottom);

            // Add a 1 pixel separator at bottom of button before the text
            contentLayout.Add(new ViewLayoutRibbonSeparator(1, false), ViewDockStyle.Bottom);

            // Add the content into the background and border
            _viewLarge.Add(contentLayout);

            // Create controller for intercepting events to determine tool tip handling
            _viewLarge.MouseController = new ToolTipController(_ribbon.TabsArea.ButtonSpecManager.ToolTipManager, 
                                                               _viewLarge, _viewLarge.MouseController);
        }

        private void CreateMediumSmallButtonView()
        {
            // Create the background and border view
            _viewMediumSmall = new ViewDrawRibbonGroupButtonBackBorder(_ribbon, GroupColorButton,
                                                                       _ribbon.StateCommon.RibbonGroupButton.PaletteBack,
                                                                       _ribbon.StateCommon.RibbonGroupButton.PaletteBorder,
                                                                       false, _needPaint)
            {
                SplitVertical = false
            };
            _viewMediumSmall.Click += OnMediumSmallButtonClick;
            _viewMediumSmall.DropDown += OnMediumSmallButtonDropDown;

            if (_ribbon.InDesignMode)
            {
                _viewMediumSmall.ContextClick += OnContextClick;
            }

            // Create the layout docker for the contents of the button
            ViewLayoutDocker contentLayout = new ViewLayoutDocker();

            // Create the image and drop down content
            _viewMediumSmallImage = new ViewDrawRibbonGroupColorButtonImage(_ribbon, GroupColorButton, false);
            _viewMediumSmallText1 = new ViewDrawRibbonGroupColorButtonText(_ribbon, GroupColorButton, true);
            _viewMediumSmallText2 = new ViewDrawRibbonGroupColorButtonText(_ribbon, GroupColorButton, false);
            _viewMediumSmallDropArrow = new ViewDrawRibbonDropArrow(_ribbon);
            _viewMediumSmallText2Sep2 = new ViewLayoutRibbonSeparator(3, false);
            _viewMediumSmallText2Sep3 = new ViewLayoutRibbonSeparator(3, false);
            ViewLayoutRibbonCenterPadding imagePadding = new ViewLayoutRibbonCenterPadding(_smallImagePadding)
            {
                _viewMediumSmallImage
            };

            // Layout the content in the center of a row
            _viewMediumSmallCenter = new ViewLayoutRibbonRowCenter
            {
                imagePadding,
                _viewMediumSmallText1,
                _viewMediumSmallText2,
                _viewMediumSmallText2Sep2,
                _viewMediumSmallDropArrow,
                _viewMediumSmallText2Sep3
            };

            // Use content as only fill item
            contentLayout.Add(_viewMediumSmallCenter, ViewDockStyle.Fill);

            // Add the content into the background and border
            _viewMediumSmall.Add(contentLayout);

            // Create controller for intercepting events to determine tool tip handling
            _viewMediumSmall.MouseController = new ToolTipController(_ribbon.TabsArea.ButtonSpecManager.ToolTipManager, 
                                                                     _viewMediumSmall, _viewMediumSmall.MouseController);
        }

        private void DefineRootView(ViewBase view)
        {
            // Remove any existing view
            Clear();

            // Use the provided view
            Add(view);

            // Provide back reference to the button definition
            GroupColorButton.ColorButtonView = view;
        }

        private void UpdateEnabledState()
        {
            // Get the correct enabled state from the button definition
            bool buttonEnabled = GroupColorButton.Enabled;
            if (GroupColorButton.KryptonCommand != null)
            {
                buttonEnabled = GroupColorButton.KryptonCommand.Enabled;
            }

            bool enabled = _ribbon.InDesignHelperMode || (buttonEnabled && _ribbon.Enabled);

            // Update enabled for the large button view
            _viewLarge.Enabled = enabled;
            _viewLargeImage.Enabled = enabled;
            _viewLargeText1.Enabled = enabled;
            _viewLargeText2.Enabled = enabled;
            _viewLargeDropArrow.Enabled = enabled;

            // Update enabled for the medium/small button view
            _viewMediumSmall.Enabled = enabled;
            _viewMediumSmallText1.Enabled = enabled;
            _viewMediumSmallText2.Enabled = enabled;
            _viewMediumSmallImage.Enabled = enabled;
            _viewMediumSmallDropArrow.Enabled = enabled;
        }

        private void UpdateCheckedState()
        {
            bool checkedState = false;

            // Only show as checked if also a check type button
            if (GroupColorButton.ButtonType == GroupButtonType.Check)
            {
                checkedState = GroupColorButton.KryptonCommand?.Checked ?? GroupColorButton.Checked;
            }

            _viewLarge.Checked = checkedState;
            _viewMediumSmall.Checked = checkedState;
        }

        private void UpdateDropDownState()
        {
            // Only show the drop down if the button is the correct type
            bool dropDown = ((GroupColorButton.ButtonType == GroupButtonType.DropDown) ||
                             (GroupColorButton.ButtonType == GroupButtonType.Split));

            // Only show text line 2 separators is a drop down is showing with no text
            bool separators = dropDown && (!string.IsNullOrEmpty(GroupColorButton.TextLine2));

            // Update large view
            _viewLargeDropArrow.Visible = dropDown;
            _viewLargeText2Sep1.Visible = separators;
            _viewLargeText2Sep2.Visible = separators;

            // Update medium/small view
            _viewMediumSmallDropArrow.Visible = dropDown;
            _viewMediumSmallText2Sep2.Visible = dropDown;
            _viewMediumSmallText2Sep3.Visible = dropDown;
        }

        private void UpdateItemSizeState()
        {
            UpdateItemSizeState(GroupColorButton.ItemSizeCurrent);
        }

        private void UpdateItemSizeState(GroupItemSize size)
        {
            _currentSize = size;

            switch (size)
            {
                case GroupItemSize.Small:
                case GroupItemSize.Medium:
                    bool show = (size == GroupItemSize.Medium);
                    _viewMediumSmallCenter.CurrentSize = size;
                    _viewMediumSmallText1.Visible = show;
                    _viewMediumSmallText2.Visible = show;
                    DefineRootView(_viewMediumSmall);
                    break;
                case GroupItemSize.Large:
                    _viewLargeCenter.CurrentSize = size;
                    DefineRootView(_viewLarge);
                    break;
            }
        }

        private void OnLargeButtonClick(object sender, EventArgs e)
        {
            GroupColorButton.PerformClick(_viewLarge.FinishDelegate);
        }

        private void OnLargeButtonDropDown(object sender, EventArgs e)
        {
            GroupColorButton.PerformDropDown(_viewLarge.FinishDelegate);
        }

        private void OnMediumSmallButtonClick(object sender, EventArgs e)
        {
            GroupColorButton.PerformClick(_viewMediumSmall.FinishDelegate);
        }

        private void OnMediumSmallButtonDropDown(object sender, EventArgs e)
        {
            GroupColorButton.PerformDropDown(_viewMediumSmall.FinishDelegate);
        }

        private void OnContextClick(object sender, MouseEventArgs e)
        {
            GroupColorButton.OnDesignTimeContextMenu(e);
        }

        private void OnButtonPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            bool updateLayout = false;
            bool updatePaint = false;

            switch (e.PropertyName)
            {
                case "SelectedColor":
                case "EmptyBorderColor":
                case "SelectedRectSmall":
                case "SelectedRectLarge":
                    _viewMediumSmallImage.SelectedColorRectChanged();
                    _viewLargeImage.SelectedColorRectChanged();
                    updatePaint = true;
                    break;
                case "Visible":
                    updateLayout = true;
                    break;
                case "TextLine1":
                    _viewLargeText1.MakeDirty();
                    _viewMediumSmallText1.MakeDirty();
                    updateLayout = true;
                    break;
                case "TextLine2":
                    _viewLargeText2.MakeDirty();
                    _viewMediumSmallText2.MakeDirty();
                    UpdateDropDownState();
                    updateLayout = true;
                    break;
                case "ButtonType":
                    UpdateDropDownState();
                    updateLayout = true;
                    break;
                case "Checked":
                    UpdateCheckedState();
                    updatePaint = true;
                    break;
                case "Enabled":
                    UpdateEnabledState();
                    updatePaint = true;
                    break;
                case "ImageLarge":
                case "ImageSmall":
                    _viewMediumSmallImage.SelectedColorRectChanged();
                    _viewLargeImage.SelectedColorRectChanged();
                    updatePaint = true;
                    break;
                case "ItemSizeMinimum":
                case "ItemSizeMaximum":
                case "ItemSizeCurrent":
                    UpdateItemSizeState();
                    updateLayout = true;
                    break;
                case "KryptonCommand":
                    _viewLargeText1.MakeDirty();
                    _viewLargeText2.MakeDirty();
                    _viewLargeImage.SelectedColorRectChanged();
                    _viewMediumSmallText1.MakeDirty();
                    _viewMediumSmallText2.MakeDirty();
                    _viewMediumSmallImage.SelectedColorRectChanged();
                    UpdateEnabledState();
                    UpdateCheckedState();
                    updateLayout = true;
                    break;
            }

            if (updateLayout)
            {
                // If we are on the currently selected tab then...
                if ((GroupColorButton.RibbonTab != null) &&
                    (_ribbon.SelectedTab == GroupColorButton.RibbonTab))
                {
                    // ...layout so the visible change is made
                    OnNeedPaint(true);
                }
            }

            if (updatePaint)
            {
                // If this button is actually defined as visible...
                if (GroupColorButton.Visible || _ribbon.InDesignMode)
                {
                    // ...and on the currently selected tab then...
                    if ((GroupColorButton.RibbonTab != null) &&
                        (_ribbon.SelectedTab == GroupColorButton.RibbonTab))
                    {
                        // ...repaint it right now
                        OnNeedPaint(false, ClientRectangle);
                    }
                }
            }
        }
        #endregion
    }
}
