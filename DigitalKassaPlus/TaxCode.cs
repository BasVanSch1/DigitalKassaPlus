using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalKassaPlus
{
    internal class TaxCode
    {
        public int Code {  get; private set; }
        public decimal Percentage { get; private set; }
        public string Description { get; private set; }

        public TaxCode(int _code, decimal _percentage, string _description)
        {
            Code = _code;
            Percentage = _percentage;
            Description = _description;
        }

        public TaxCode(int _code, decimal _percentage)
        {
            new TaxCode(_code, _percentage, "");
        }

        public TaxCode(int _code)
        {
            new TaxCode(_code, 0.10m, "10 procent BTW"); // uiteindelijk BTW code ophalen uit database
        }

        public bool SetPercentage(decimal _percentage)
        {
            if (_percentage >= 0 && _percentage <= 100)
            {
                Percentage = _percentage;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set the description of this TaxCode
        /// </summary>
        /// <param name="_description">Description of this TaxCode</param>
        /// <returns>boolean value of true if it succeeds, otherwise false.</returns>
        public bool SetDescription(string _description)
        {
            try
            {
                Description = _description;
            } catch (Exception e)
            {
                Console.WriteLine($"Error occurred while setting the description on TaxCode: (Code: {Code}, Percentage: {Percentage})\n" +
                    $"{e.Message}");
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"TaxCode: {Code}, Percentage: {Percentage}, Description: {Description}.";
        }
    }
}
