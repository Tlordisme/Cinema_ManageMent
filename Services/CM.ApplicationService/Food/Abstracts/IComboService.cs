using CM.Dtos.Food;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Food.Abstracts
{
    public interface IComboService
    {
        string CreateCombo(AddOrUpdateComboDto comboDto);
        void UpdateCombo(ComboDto comboDto);
        void DeleteCombo(string comboId);
        ComboDto GetComboById(string comboId);
        List<ComboDto> GetAllCombos();
    }
}
