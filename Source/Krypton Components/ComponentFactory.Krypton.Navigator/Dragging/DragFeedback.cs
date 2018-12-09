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
using System.Diagnostics;
using ComponentFactory.Krypton.Toolkit;

namespace ComponentFactory.Krypton.Navigator
{
    /// <summary>
    /// Base class for drag feedback implementations.
    /// </summary>
    public abstract class DragFeedback : IDisposable
    {
        #region Instance Fields

        #endregion

        #region Identity
        /// <summary>
		/// Release resources.
		/// </summary>
        ~DragFeedback()
		{
			// Only dispose of resources once
			if (!IsDisposed)
			{
				// Only dispose of managed resources
				Dispose(false);
			}
		}

		/// <summary>
		/// Release managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			// Only dispose of resources once
			if (!IsDisposed)
			{
				// Dispose of managed and unmanaged resources
				Dispose(true);
			}
		}

		/// <summary>
		/// Release unmanaged and optionally managed resources.
		/// </summary>
		/// <param name="disposing">Called from Dispose method.</param>
		protected virtual void Dispose(bool disposing)
		{
			// If called from explicit call to Dispose
			if (disposing)
			{
				// No need to call destructor once dispose has occured
				GC.SuppressFinalize(this);

                PageDragEndData = null;
                DragTargets = null;
			}

			// Mark as disposed
			IsDisposed = true;
		}

        /// <summary>
        /// Gets a value indicating if the view has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        #endregion

        #region Public
        /// <summary>
        /// Called to initialize the implementation when dragging starts.
        /// </summary>
        /// <param name="paletteDragDrop">Drawing palette.</param>
        /// <param name="renderer">Drawing renderer.</param>
        /// <param name="pageDragEndData">Drag data associated with drag operation.</param>
        /// <param name="dragTargets">List of all drag targets.</param>
        public virtual void Start(IPaletteDragDrop paletteDragDrop,
                                  IRenderer renderer,
                                  PageDragEndData pageDragEndData, 
                                  DragTargetList dragTargets)
        {
            Debug.Assert(paletteDragDrop != null);
            Debug.Assert(renderer != null);
            Debug.Assert(pageDragEndData != null);
            Debug.Assert(dragTargets != null);

            PaletteDragDrop = paletteDragDrop;
            Renderer = renderer;
            PageDragEndData = pageDragEndData;
            DragTargets = dragTargets;
        }

        /// <summary>
        /// Called to request feedback be shown for the specified target.
        /// </summary>
        /// <param name="screenPt">Current screen point of mouse.</param>
        /// <param name="target">Target that needs feedback.</param>
        /// <returns>Updated drag target.</returns>
        public abstract DragTarget Feedback(Point screenPt, DragTarget target);

        /// <summary>
        /// Called to cleanup when dragging has finished.
        /// </summary>
        public virtual void Quit()
        {
            PageDragEndData = null;
            DragTargets = null;
        }
        #endregion

        #region Protected
        /// <summary>
        /// Gets access to the cached drawing palette.
        /// </summary>
        protected IPaletteDragDrop PaletteDragDrop { get; private set; }

        /// <summary>
        /// Gets access to the cached drawing renderer.
        /// </summary>
        protected IRenderer Renderer { get; private set; }

        /// <summary>
        /// Gets access to the cached drag data.
        /// </summary>
        protected PageDragEndData PageDragEndData { get; private set; }

        /// <summary>
        /// Gets access to the cached drag target list.
        /// </summary>
        protected DragTargetList DragTargets { get; private set; }

        #endregion
    }
}
