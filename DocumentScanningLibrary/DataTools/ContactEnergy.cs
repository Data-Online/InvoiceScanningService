using DocumentScanningLibrary.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FuzzyString;


// To manage logic around Contact Energy bills
// Code specific to bill type

namespace DocumentScanningLibrary.DataTools
{
    class ContactEnergy
    {
        private string _scannedText = "";
        private string _companyName = "contact";

        public ContactEnergy(string ScannedText)
        {
            _scannedText = ScannedText.TrimAndReduce();
        }

        public ScannedRecord ProcessContactInvoice()
        {
            int failedMatches = 0; int failedValidations = 0; string failedValidationRecords = string.Empty;
            ScannedRecord scannedRecord = new ScannedRecord();
            // Logic for Contact invoice type.

            ScanDate(ref scannedRecord, ref failedMatches);
            ScanFixedData(ref scannedRecord, ref failedMatches);
            ScanLineCharges(ref scannedRecord, ref failedMatches);
            ScanEnergyCharges(ref scannedRecord, ref failedMatches);
            ScanOtherCharges(ref scannedRecord, ref failedValidations);
            ValidateScannedData(ref scannedRecord, ref failedValidations, ref failedValidationRecords);

            scannedRecord.CompanyName = "to be advised"; // This is currently a "not null" field. Either scan in or work out from account number? GPA ***
            scannedRecord.FailedMatches = failedMatches;
            scannedRecord.FailedValidations = failedValidations;
            scannedRecord.FailedValidationRecords = failedValidationRecords;
            return scannedRecord;
        }



        private bool ValueWithinRange(decimal? valueA, decimal? valueB, ref int failedValidations)
        {
            bool _result = false;
            decimal _tolerance = 1.00M; // **GPA, use a percentage of value?
            if (valueA.HasValue & valueB.HasValue)
            {
                _result = (valueA >= (valueB - _tolerance) & valueA <= (valueB + _tolerance));
                //return (valueA >= (valueB - _tolerance) & valueA <= (valueB + _tolerance));
            }
            else
            {
                _result = false;
            }
            if (!_result)
                failedValidations++;
            return _result;
        }

        #region Handle specific sections of the scanned page of data
        private void ValidateScannedData(ref ScannedRecord scannedRecord, ref int failedValidations, ref string failedValidationRecords)
        {
            // Validate the data based on expected calculations
            if (!ValueWithinRange(((scannedRecord.Weekday00_04_Units + scannedRecord.Weekday00_04_Losses) * scannedRecord.Weekday00_04_CostPerUnit), (scannedRecord.Weekday00_04_Charge * 100), ref failedValidations))
                failedValidationRecords+= "Weekday00_04:";
            if (!ValueWithinRange(((scannedRecord.Weekday04_08_Units + scannedRecord.Weekday04_08_Losses) * scannedRecord.Weekday04_08_CostPerUnit), (scannedRecord.Weekday04_08_Charge * 100), ref failedValidations))
                failedValidationRecords += "Weekday08_12:";
            if (!ValueWithinRange(((scannedRecord.Weekday08_12_Units + scannedRecord.Weekday08_12_Losses) * scannedRecord.Weekday08_12_CostPerUnit), (scannedRecord.Weekday08_12_Charge * 100), ref failedValidations))
                failedValidationRecords += "Weekday08_12:";
            if (!ValueWithinRange(((scannedRecord.Weekday12_16_Units + scannedRecord.Weekday12_16_Losses) * scannedRecord.Weekday12_16_CostPerUnit), (scannedRecord.Weekday12_16_Charge * 100), ref failedValidations))
                failedValidationRecords += "Weekday12_16:";
            if (!ValueWithinRange(((scannedRecord.Weekday16_20_Units + scannedRecord.Weekday16_20_Losses) * scannedRecord.Weekday16_20_CostPerUnit), (scannedRecord.Weekday16_20_Charge * 100), ref failedValidations))
                failedValidationRecords += "Weekday16_20:";
            if (!ValueWithinRange(((scannedRecord.Weekday20_24_Units + scannedRecord.Weekday20_24_Losses) * scannedRecord.Weekday20_24_CostPerUnit), (scannedRecord.Weekday20_24_Charge * 100), ref failedValidations))
                failedValidationRecords += "Weekday20_24:";

            if (!ValueWithinRange(((scannedRecord.Weekend00_04_Units + scannedRecord.Weekend00_04_Losses) * scannedRecord.Weekend00_04_CostPerUnit), (scannedRecord.Weekend00_04_Charge * 100), ref failedValidations))
                failedValidationRecords += "Weekend00_04:";
            if (!ValueWithinRange(((scannedRecord.Weekend04_08_Units + scannedRecord.Weekend04_08_Losses) * scannedRecord.Weekend04_08_CostPerUnit), (scannedRecord.Weekend04_08_Charge * 100), ref failedValidations))
                failedValidationRecords += "Weekend04_08:";
            if (!ValueWithinRange(((scannedRecord.Weekend08_12_Units + scannedRecord.Weekend08_12_Losses) * scannedRecord.Weekend08_12_CostPerUnit), (scannedRecord.Weekend08_12_Charge * 100), ref failedValidations))
                failedValidationRecords += "Weekend08_12:";
            if (!ValueWithinRange(((scannedRecord.Weekend12_16_Units + scannedRecord.Weekend12_16_Losses) * scannedRecord.Weekend12_16_CostPerUnit), (scannedRecord.Weekend12_16_Charge * 100), ref failedValidations))
                failedValidationRecords += "Weekend12_16:";
            if (!ValueWithinRange(((scannedRecord.Weekend16_20_Units + scannedRecord.Weekend16_20_Losses) * scannedRecord.Weekend16_20_CostPerUnit), (scannedRecord.Weekend16_20_Charge * 100), ref failedValidations))
                failedValidationRecords += "Weekend16_20:";
            if (!ValueWithinRange(((scannedRecord.Weekend20_24_Units + scannedRecord.Weekend20_24_Losses) * scannedRecord.Weekend20_24_CostPerUnit), (scannedRecord.Weekend20_24_Charge * 100), ref failedValidations))
                failedValidationRecords += "Weekend20_24:";

            if (!ValueWithinRange((scannedRecord.LCfixedUnits * scannedRecord.LCfixedCostPerUnit), scannedRecord.LCfixedCharge, ref failedValidations))  // Rounding will result in error here
                failedValidationRecords += "LCfixed:";
            if (!ValueWithinRange((scannedRecord.LCmaxDemandUnits * scannedRecord.LCmaxDemandCostPerUnit), (scannedRecord.LCmaxDemandCharge), ref failedValidations))
                failedValidationRecords += "LCmaxDemand:";
            // GPA*** Note that this value (1000) is from the actual bill. May need to scan this in in future
            if (!ValueWithinRange((scannedRecord.LCassessedCapacityUnits * scannedRecord.LCassessedCapacityPerUnit * 1000), (scannedRecord.LCassessedCapacityCharge * 100), ref failedValidations))
                failedValidationRecords += "LCassessedCapacity:";

            // Other Charges
            if (!ValueWithinRange((scannedRecord.AdminCharge_Units * scannedRecord.AdminCharge_Rate), (scannedRecord.AdminCharge_Charge), ref failedValidations))
                failedValidationRecords += "AdminCharge:";
            if (!ValueWithinRange((scannedRecord.ECLevies_Units * scannedRecord.ECLevies_Rate), (scannedRecord.ECLevies_Charge * 100), ref failedValidations))
                failedValidationRecords += "ECLevies:";

            //decimal _total = 0.00M;
            //_total = scannedRecord.Weekday00_04_Charge + scannedRecord.Weekday04_08_Charge;


        }
        private void ScanDate(ref ScannedRecord scannedRecord, ref int failedMatches)
        {

            Regex _regex = new Regex(@"(From).*(Days)");
            var dt = ReturnDate(_regex.Match(_scannedText).ToString().RemoveString("From").RemoveString("Days"), ref failedMatches);

            if (dt.Count() == 2)
            {
                scannedRecord.FromDate = dt[0];
                scannedRecord.ToDate = dt[1];
            }
            else
                failedMatches++;
        }
        private void ScanFixedData(ref ScannedRecord scannedRecord, ref int failedMatches)
        {
            Regex _regex = new Regex(@"(Account)\s+(Number:)\s+\w+");
            scannedRecord.AccountNumber = ReturnText(_regex.Match(_scannedText).ToString(), ':', ref failedMatches, 1);
            _regex = new Regex(@"(Invoice)\s+(Number:)\s+\w+");
            scannedRecord.InvoiceNumber = ReturnText(_regex.Match(_scannedText).ToString(), ':', ref failedMatches, 1);
            _regex = new Regex(@"(ICP).*(Fixed)");
            scannedRecord.ICPnumber = ReturnText(_regex.Match(_scannedText).ToString(), ' ', ref failedMatches, 2).RemoveSymbols();
        }
        private void ScanLineCharges(ref ScannedRecord scannedRecord, ref int failedMatches)
        {
            int _unitsField = 1; int _costPerUnitField = 2; int _chargeField = 3;
            string _resultString = "";
            Regex _regex = new Regex(@"(Fixed Line Charge).*(Variable Line Charge)");
            //_resultString = _regex.Match(_scannedText).ToString().OnlyDigits().TrimAndReduce();
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.LCfixedUnits = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField, 0);
            scannedRecord.LCfixedCostPerUnit = ReturnDecimal(_resultString, ' ', ref failedMatches, _costPerUnitField);
            scannedRecord.LCfixedCharge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);

            _regex = new Regex(@"(Variable Line Charge).*(Anytime Maximum Demand)");
            //_resultString = _regex.Match(_scannedText).ToString().OnlyDigits().TrimAndReduce();
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.LCvariableUnits = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField);
            scannedRecord.LCvariableCostPerUnit = ReturnDecimal(_resultString, ' ', ref failedMatches, _costPerUnitField, 3);
            scannedRecord.LCvariableCharge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);

            _regex = new Regex(@"(Anytime Maximum Demand).*(Assessed Capacity)");
            //_resultString = _regex.Match(_scannedText).ToString().OnlyDigits().TrimAndReduce();
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.LCmaxDemandUnits = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField, 1);
            scannedRecord.LCmaxDemandCostPerUnit = ReturnDecimal(_resultString, ' ', ref failedMatches, _costPerUnitField);
            scannedRecord.LCmaxDemandCharge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);

            _regex = new Regex(@"(Assessed Capacity).*(Total Line)");
            _costPerUnitField = 3; _chargeField = 99; // When 99 - returns the last number form input string
            //_resultString = _regex.Match(_scannedText).ToString().RemoveKnownSymbols().OnlyDigits().TrimAndReduce();
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.LCassessedCapacityUnits = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField, 0);
            scannedRecord.LCassessedCapacityPerUnit = ReturnDecimal(_resultString, ' ', ref failedMatches, _costPerUnitField);
            scannedRecord.LCassessedCapacityCharge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);
        }
        private void ScanOtherCharges(ref ScannedRecord scannedRecord, ref int failedMatches)
        {
            int _unitsField = 0; int _rateField = 1; int _chargeField = 2; int _totalChargeField = 99;
            string _resultString = "";
            Regex _regex = new Regex(@"(Administration).*(Electricity)");
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.AdminCharge_Units = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField, 0);
            scannedRecord.AdminCharge_Rate = ReturnDecimal(_resultString, ' ', ref failedMatches, _rateField);
            scannedRecord.AdminCharge_Charge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);

            _regex = new Regex(@"(Electricity)\s\w*\s(Levies).*$");
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.ECLevies_Units = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField);
            scannedRecord.ECLevies_Rate = ReturnDecimal(_resultString, ' ', ref failedMatches, _rateField, 3);
            scannedRecord.ECLevies_Charge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);

            scannedRecord.TotalCharge = ReturnDecimal(_resultString, ' ', ref failedMatches, _totalChargeField);
        }
        private void ScanEnergyCharges(ref ScannedRecord scannedRecord, ref int failedMatches)
        {
            int _unitsField = 2; int _lossesField = 3; int costPerUnitField = 4; int _chargeField = 5;
            string _resultString = "";
            Regex _regex = new Regex(@"(Weekdays \x280000).*(Weekdays \x280400)");
            //_resultString = _regex.Match(_scannedText).ToString().OnlyDigits().TrimAndReduce();
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.Weekday00_04_Units = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField);
            scannedRecord.Weekday00_04_Losses = ReturnDecimal(_resultString, ' ', ref failedMatches, _lossesField);
            scannedRecord.Weekday00_04_CostPerUnit = ReturnDecimal(_resultString, ' ', ref failedMatches, costPerUnitField, 3);
            scannedRecord.Weekday00_04_Charge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);

            _regex = new Regex(@"(Weekdays \x280400).*(Weekdays \x280800)");
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.Weekday04_08_Units = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField);
            scannedRecord.Weekday04_08_Losses = ReturnDecimal(_resultString, ' ', ref failedMatches, _lossesField);
            scannedRecord.Weekday04_08_CostPerUnit = ReturnDecimal(_resultString, ' ', ref failedMatches, costPerUnitField, 3);
            scannedRecord.Weekday04_08_Charge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);

            _regex = new Regex(@"(Weekdays \x280800).*(Weekdays \x281200)");
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.Weekday08_12_Units = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField);
            scannedRecord.Weekday08_12_Losses = ReturnDecimal(_resultString, ' ', ref failedMatches, _lossesField);
            scannedRecord.Weekday08_12_CostPerUnit = ReturnDecimal(_resultString, ' ', ref failedMatches, costPerUnitField, 3);
            scannedRecord.Weekday08_12_Charge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);

            _regex = new Regex(@"(Weekdays \x281200).*(Weekdays \x281600)");
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.Weekday12_16_Units = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField);
            scannedRecord.Weekday12_16_Losses = ReturnDecimal(_resultString, ' ', ref failedMatches, _lossesField);
            scannedRecord.Weekday12_16_CostPerUnit = ReturnDecimal(_resultString, ' ', ref failedMatches, costPerUnitField, 3);
            scannedRecord.Weekday12_16_Charge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);

            _regex = new Regex(@"(Weekdays \x281600).*(Weekdays \x282000)");
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.Weekday16_20_Units = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField);
            scannedRecord.Weekday16_20_Losses = ReturnDecimal(_resultString, ' ', ref failedMatches, _lossesField);
            scannedRecord.Weekday16_20_CostPerUnit = ReturnDecimal(_resultString, ' ', ref failedMatches, costPerUnitField, 3);
            scannedRecord.Weekday16_20_Charge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);

            _regex = new Regex(@"(Weekdays \x282000).*(Energy Totals)");
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.Weekday20_24_Units = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField);
            scannedRecord.Weekday20_24_Losses = ReturnDecimal(_resultString, ' ', ref failedMatches, _lossesField);
            scannedRecord.Weekday20_24_CostPerUnit = ReturnDecimal(_resultString, ' ', ref failedMatches, costPerUnitField, 3);
            scannedRecord.Weekday20_24_Charge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);

            /// Weekend
            _regex = new Regex(@"(Weekends \x280000).*(Weekends \x280400)");
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.Weekend00_04_Units = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField);
            scannedRecord.Weekend00_04_Losses = ReturnDecimal(_resultString, ' ', ref failedMatches, _lossesField);
            scannedRecord.Weekend00_04_CostPerUnit = ReturnDecimal(_resultString, ' ', ref failedMatches, costPerUnitField, 3);
            scannedRecord.Weekend00_04_Charge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);

            _regex = new Regex(@"(Weekends \x280400).*(Weekends \x280800)");
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.Weekend04_08_Units = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField);
            scannedRecord.Weekend04_08_Losses = ReturnDecimal(_resultString, ' ', ref failedMatches, _lossesField);
            scannedRecord.Weekend04_08_CostPerUnit = ReturnDecimal(_resultString, ' ', ref failedMatches, costPerUnitField, 3);
            scannedRecord.Weekend04_08_Charge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);

            _regex = new Regex(@"(Weekends \x280800).*(Weekends \x281200)");
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.Weekend08_12_Units = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField);
            scannedRecord.Weekend08_12_Losses = ReturnDecimal(_resultString, ' ', ref failedMatches, _lossesField);
            scannedRecord.Weekend08_12_CostPerUnit = ReturnDecimal(_resultString, ' ', ref failedMatches, costPerUnitField, 3);
            scannedRecord.Weekend08_12_Charge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);

            _regex = new Regex(@"(Weekends \x281200).*(Weekends \x281600)");
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.Weekend12_16_Units = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField);
            scannedRecord.Weekend12_16_Losses = ReturnDecimal(_resultString, ' ', ref failedMatches, _lossesField);
            scannedRecord.Weekend12_16_CostPerUnit = ReturnDecimal(_resultString, ' ', ref failedMatches, costPerUnitField, 3);
            scannedRecord.Weekend12_16_Charge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);

            _regex = new Regex(@"(Weekends \x281600).*(Weekends \x282000)");
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.Weekend16_20_Units = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField);
            scannedRecord.Weekend16_20_Losses = ReturnDecimal(_resultString, ' ', ref failedMatches, _lossesField);
            scannedRecord.Weekend16_20_CostPerUnit = ReturnDecimal(_resultString, ' ', ref failedMatches, costPerUnitField, 3);
            scannedRecord.Weekend16_20_Charge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);

            _regex = new Regex(@"(Weekends \x282000).*(Energy Totals)");
            _resultString = _regex.Match(_scannedText).ToString().ReduceForDecimals();
            scannedRecord.Weekend20_24_Units = ReturnDecimal(_resultString, ' ', ref failedMatches, _unitsField);
            scannedRecord.Weekend20_24_Losses = ReturnDecimal(_resultString, ' ', ref failedMatches, _lossesField);
            scannedRecord.Weekend20_24_CostPerUnit = ReturnDecimal(_resultString, ' ', ref failedMatches, costPerUnitField, 3);
            scannedRecord.Weekend20_24_Charge = ReturnDecimal(_resultString, ' ', ref failedMatches, _chargeField);
        }

        #endregion

        private decimal ReturnDecimal(string matchedText, char splitAt, ref int failedMatches, int fieldNo, int decimalPlaces = 2)
        {
            // Returns decimal for specified field number to defined decimal places. Decimal points to be removed from text passed
            //
            string _digits = "";
            decimal returnValue = 0.000M;
            try
            {
                //_digits = ReturnText(matchedText.RemoveKnownSymbols().RemoveSymbols(), splitAt, ref failedMatches, fieldNo);
                _digits = ReturnText(matchedText, splitAt, ref failedMatches, fieldNo);
                if (decimalPlaces > 0)
                {
                    _digits = String.Format("{0}.{1}", _digits.Substring(0, _digits.Length - decimalPlaces), _digits.Substring(_digits.Length - decimalPlaces, decimalPlaces));
                }
                returnValue = Convert.ToDecimal(_digits);
                // returnValue = Convert.ToDecimal(ReturnText(matchedText.TrimAndReduce().RemoveSymbols(), splitAt, ref failedMatches, fieldNo));
            }
            catch
            {
                failedMatches++;
            }
            return returnValue;
        }


        private string ReturnText(string matchedText, char splitAt, ref int failedMatches, int fieldNo)
        {
            int _getFieldNo;
            string _returnValue = "";
            if (fieldNo > Convert.ToString(matchedText).Split(splitAt).Length-1)
                _getFieldNo = Convert.ToString(matchedText).Split(splitAt).Length-1;
            else
                _getFieldNo = fieldNo;
            try
            {
                _returnValue = Convert.ToString(matchedText).Split(splitAt)[_getFieldNo];
            }
            catch
            {
                failedMatches++;
            }
            return _returnValue;
        }

        private DateTime[] ReturnDate(string str, ref int failedMatches)
        {
            //str = str.TrimAndReduce();

            IFormatProvider _culture = new System.Globalization.CultureInfo("en-NZ", true);
            DateTime _seedDate = Convert.ToDateTime(String.Format("{0}/{1}/{2}", "01", "01", "2000"), _culture);
            DateTime _fromDate = _seedDate;
            DateTime _toDate = _seedDate;
            DateTime[] returnDates = new DateTime[] { _fromDate, _toDate };

            Object[] test = { _fromDate, _toDate, 0 };

            string _testString = "";

            _testString = str.Substring(0, str.IndexOf(" to ")).RemoveString(" to ").Trim();
            string _fromYear = _testString.YearNumber();
            string _fromMonth = _testString.ToLower().DigitsToText().OnlyAlpha().RemoveSingleAlphaCharacters().Trim().MonthNumberFromName();
            string _fromDay = _testString.DayNumber();

            _testString = str.Substring(str.IndexOf(" to ")).RemoveString(" to ").Trim();
            string _toYear = _testString.YearNumber();
            string _toMonth = _testString.ToLower().DigitsToText().OnlyAlpha().RemoveSingleAlphaCharacters().Trim().MonthNumberFromName();
            string _toDay = _testString.DayNumber();

            Regex _regex = new Regex(@"\d{1,2}$");
            string _noDays = _regex.Match(_testString).ToString();
            //test[2] = _noDays; 

            FixDates(ref _fromYear, ref _fromMonth, ref _toYear, ref _toMonth);

            try
            {
                _fromDate = Convert.ToDateTime(String.Format("{0}/{1}/{2}", _fromDay, _fromMonth, _fromYear), _culture);
            }
            catch
            {
                failedMatches++;
            }
            try
            {
                _toDate = Convert.ToDateTime(String.Format("{0}/{1}/{2}", _toDay, _toMonth, _toYear), _culture);
            }
            catch
            {
                failedMatches++;
            }

            if (_toDate == _seedDate & _fromDate != _seedDate) // Can use number of days to calculate
                _toDate = FixDates(_fromDate, _toDate, Convert.ToInt32(_noDays) - 1);
            else if (_fromDate == _seedDate & _toDate != _seedDate)
                _fromDate = FixDates(_toDate, _fromDate, Convert.ToInt32(_noDays) - 1 * -1);

            returnDates[0] = _fromDate;
            returnDates[1] = _toDate;

            return returnDates;
        }

        private DateTime FixDates(DateTime haveDate, DateTime noDate, int noDays)
        {
            noDate = haveDate;
            return noDate.AddDays(noDays);
        }

        private void FixDates(ref string yearA, ref string monthA, ref string yearB, ref string monthB)
        {
            // Can add more sophisticated logic later
            if (yearA.Length == 0 & monthA == monthB & monthA.Length > 0)
            {
                // Assume same year
                yearA = yearB;
            }
            else if (yearB.Length == 0 & monthA == monthB & monthA.Length > 0)
            {
                yearB = yearA;
            }
        }

    }

    public static class StringExtension
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.

        public static string ReduceForDecimals(this string str)
        {
            return str.RemoveWords().TextToDigits().OnlyDigits().RemoveSingleAlphaCharacters().RemoveSingleDigits().TrimAndReduce();
        }
        public static string TrimAndReduce(this string str)
        {
            return ConvertWhitespacesToSingleSpaces(str).Trim();
        }

        public static string ConvertWhitespacesToSingleSpaces(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }

        public static string DigitsToText(this string inputString)
        {
            // Convert digits to text
            string outputString = inputString.ToLower();
            outputString = outputString.Replace("0", "o");
            // Add others here --> 
            return outputString;
        }

        public static string TextToDigits(this string inputString)
        {
            // Convert digits to text
            string outputString = inputString.ToLower();
            outputString = outputString.Replace("i", "1");
            outputString = outputString.Replace("o", "0");
            // Add others here --> 
            return outputString;
        }

        public static string RemoveKnownSymbols(this string inputString)
        {
            // Specific sequences to remove from input string.
            string outputString = inputString;
            // Contact energy bills:
            outputString = outputString.Replace("c", "");       // Cents
            outputString = outputString.Replace("day22", "");   // per day22
            // Add others here --> 
            return outputString;
        }
        public static string RemoveWords(this string inputString, int minChars = 3)
        {
            Regex _regex = new Regex(@"[a-zA-Z]{"+minChars.ToString()+@",}");
            return _regex.Replace(inputString, " ");
        }
        public static string RemoveSymbols(this string inputString)
        {
            // Used as a precursor to source data. Remove all symbols from the scanned text
            Regex _regex = new Regex(@"[^a-zA-Z0-9 ]");
            return _regex.Replace(inputString, "");
        }

        public static string RemoveString(this string inputString, string str)
        {
            return inputString.Replace(str, "");
        }

        public static string MonthNumberFromName(this string str)
        {
            // Return date number of month 

            // Reduce string to minimum required
            string[] _monthNames = { "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" };

            int _monthNumber = Array.IndexOf(_monthNames, str) + 1;
            if (_monthNumber == 0)
            {
                // Not found, try fuzzy matching
                foreach (string monthName in _monthNames)
                {
                    if (FuzzyMatch(str, monthName))
                    {
                        // found with fuzzy match!
                        _monthNumber = Array.IndexOf(_monthNames, monthName) + 1;
                        break;
                    }
                }
            }
            return _monthNumber.ToString().Trim();
        }

        ////public static bool FuzzyMatchOnPossibleMatch(string source, string target, int lcsLength)
        ////{
        ////    // lcsLength = number of characters in longest common substring
        ////    if ( target.LongestCommonSubstring(source).Length >= lcsLength ) // Need to find at least lcsLength characters in the target string
        ////    { 
        ////        //int _index = source.IndexOf(target.LongestCommonSubstring(source));
        ////        //if (_index > 1) // Then possible match here
        ////            return FuzzyMatch(source, target); // Need to make sure length not longer than the string
        ////    }
        ////    return false;
        ////}

        public static bool FuzzyMatch(string source, string target)
        {
            List<FuzzyStringComparisonOptions> options = new List<FuzzyStringComparisonOptions>();
            options.Add(FuzzyStringComparisonOptions.UseOverlapCoefficient);
            //options.Add(FuzzyStringComparisonOptions.UseLongestCommonSubsequence);
            //options.Add(FuzzyStringComparisonOptions.UseLongestCommonSubstring);
            FuzzyStringComparisonTolerance tolerance = FuzzyStringComparisonTolerance.Normal; //.Strong;

            return source.ApproximatelyEquals(target, options, tolerance);
        }

        public static string OnlyAlpha(this string str)
        {
            Regex rgx = new Regex("[^a-zA-Z ]");
            return rgx.Replace(str, "");
        }

        public static string OnlyDigits(this string str)
        {
            Regex rgx = new Regex("[^0-9 ]");
            return rgx.Replace(str, "");
        }

        public static string OnlyAlphaNumeric(this string str)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 ]");
            return rgx.Replace(str, "");
        }

        public static string RemoveSingleAlphaCharacters(this string str)
        {
            // Remove any single alphanumeric characters found in the source string.
            //Regex rgx = new Regex(@"\s.\s|\s.$|^.\s");
            //Regex rgx = new Regex(@"\s\w\s|\s\w$|^\w\s");
            string _returnString = str;
            Regex rgx = new Regex(@"\s[a-zA-Z]\s|\s[a-zA-Z]$|^[a-zA-Z]\s", RegexOptions.Compiled);
            while (rgx.IsMatch(_returnString))
            {
                _returnString = rgx.Replace(_returnString, " ");
            }
            return _returnString;
        }
        public static string RemoveSingleDigits(this string str)
        {
            // Remove any single alphanumeric characters found in the source string.
            string _returnString = str;
            Regex rgx = new Regex(@"\s\d\s|\s\d$|^\d\s", RegexOptions.Compiled);
            while (rgx.IsMatch(_returnString))
            {
                _returnString = rgx.Replace(_returnString, " ");
            }
            return _returnString;
        }
        public static string DayNumber(this string str)
        {
            // Return day number
            str = str.TextToDigits().Trim();
            string dayNumber = "0";
            Regex _regex = new Regex(@"^\d{1,2}\s{1}");
            try
            {
                dayNumber = _regex.Match(str).ToString();
            }
            catch
            {
            }
            return dayNumber.Trim();
        }

        public static string YearNumber(this string str)
        {
            //string year = str.TextToDigits().Substring(str.IndexOf("20"), 4).ToString();

            // Return year number of month 
            // Look for dates starting 20xx(20)
            str = str.TextToDigits();
            string yearNumber = "2000";
            Regex _regex = new Regex(@"(20)\d{2}");
            try
            {
                yearNumber = _regex.Match(str).ToString();
            }
            catch
            {
            }
            return yearNumber.Trim();
        }

    }
}
