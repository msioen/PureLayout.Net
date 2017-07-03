﻿using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using GalaSoft.MvvmLight.Helpers;
using PureLayout.Net;
using PureLayoutSample.ViewModels;
using UIKit;

namespace PureLayoutSample
{
    class RootViewController : UIViewController
    {
        private UITextField _amount;
        private UITextField _tipPercentage;
        private UIButton _calculateButton;
        private UITableView _tableView;
        private UIButton _clearHistoryButton;
        private List<Binding> _bindings = new List<Binding>();

        public RootViewController()
        {
        }
        public RootViewController(IntPtr handler):base(handler)
        {
        }

        public TipCalculatorViewModel ViewModel => ViewModelLocator.Instance.TipCalculatorViewModel;


        #region Overrides of UIViewController

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            LayoutView();
            CreateBindings();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            _bindings.ForEach(b => b.Detach());
            _bindings.Clear();
        }

        #endregion

        private void LayoutView()
        {
            View.BackgroundColor = UIColor.White;
            _amount = new UITextField
            {
                KeyboardType = UIKeyboardType.DecimalPad,
                Placeholder = "Total Amount",
                BorderStyle = UITextBorderStyle.RoundedRect
            };

            _tipPercentage = new UITextField
            {
                KeyboardType = UIKeyboardType.DecimalPad,
                Text = "10.0",
                BorderStyle = UITextBorderStyle.RoundedRect
            };
            _calculateButton = new UIButton(UIButtonType.RoundedRect);
            _calculateButton.SetTitle("Calculate Tip", UIControlState.Normal);
            _tableView = new UITableView();
            _clearHistoryButton = new UIButton(UIButtonType.RoundedRect);
            _clearHistoryButton.SetTitle("Clear History", UIControlState.Normal);
            _clearHistoryButton.BackgroundColor = UIColor.FromRGB(243, 105, 105);
            _clearHistoryButton.SetTitleColor(UIColor.White, UIControlState.Normal);

            View.Add(_amount);
            View.Add(_tipPercentage);
            View.Add(_calculateButton);
            View.Add(_tableView);
            View.Add(_clearHistoryButton);

            _amount.AutoPinEdgeToSuperviewEdge(ALEdge.Top, Constants.WideMargin);
            _amount.AutoPinEdgeToSuperviewEdge(ALEdge.Left, Constants.DefaultMargin);
            _amount.AutoPinEdge(ALEdge.Trailing, ALEdge.Leading, _tipPercentage, -Constants.DefaultMargin);

            _tipPercentage.AutoPinEdgeToSuperviewEdge(ALEdge.Right, Constants.DefaultMargin);
            _tipPercentage.AutoAlignAxis(ALAxis.Horizontal, _amount);

            _calculateButton.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, _amount, Constants.DefaultMargin);
            _calculateButton.AutoAlignAxisToSuperviewAxis(ALAxis.Vertical);

            _tableView.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, _calculateButton, Constants.WideMargin);
            _tableView.AutoPinEdgeToSuperviewEdge(ALEdge.Leading, Constants.DefaultMargin);
            _tableView.AutoPinEdgeToSuperviewEdge(ALEdge.Trailing, Constants.DefaultMargin);
            _tableView.AutoPinEdge(ALEdge.Bottom, ALEdge.Top, _clearHistoryButton);

            _clearHistoryButton.AutoPinEdgesToSuperviewEdgesExcludingEdge(ALEdge.Top);
            _clearHistoryButton.AutoSetDimension(ALDimension.Height, Constants.WideMargin * 2);
            _clearHistoryButton.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, _tableView);
        }

        private void CreateBindings()
        {
            _bindings.Add(View.SetBinding(() => ViewModel.CurrentPrice, () => _amount.Text, BindingMode.TwoWay));
            _bindings.Add(View.SetBinding(() => ViewModel.CurrentTipPercentage, () => _tipPercentage.Text, BindingMode.TwoWay));
            _tableView.Source = ViewModel.TipHistory.GetTableViewSource(CreateTipCell, BindCellDelegate);
            _calculateButton.SetCommand(ViewModel.CalculateTipCommand);
            _clearHistoryButton.SetCommand(ViewModel.ClearTipHistoryCommand);
        }

        private UITableViewCell CreateTipCell(NSString cellIdentifier)
        {
            return new UITableViewCell(UITableViewCellStyle.Default, "Tip");
        }

        private void BindCellDelegate(UITableViewCell cell, string tip, NSIndexPath path)
        {
            cell.TextLabel.Text = tip;
        }
    }

    public static class Constants
    {
        public const long TightMargin = DefaultMargin / 2;
        public const long DefaultMargin = 16;
        public const long WideMargin = DefaultMargin * 2;
    }
}