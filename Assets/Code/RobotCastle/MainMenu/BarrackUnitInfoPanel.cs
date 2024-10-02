using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.Saving;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public class BarrackUnitInfoPanel : MonoBehaviour
    {
        [SerializeField] private Pool _pool;
        private BarrackUnitInfo[,] uiGrid ;
        

        public void Init(Vector2Int gridSize)
        {
            _pool.Init();
            uiGrid = new BarrackUnitInfo[gridSize.x, gridSize.y];
            for (var y = 0; y < gridSize.y; y++)
            {
                for (var x = 0; x < gridSize.x; x++)
                {
                    var ui  = (BarrackUnitInfo)_pool.GetOne();
                    uiGrid[x, y] = ui;
                    ui.gameObject.SetActive(false);
                }                
            }
        }

        public void ShowForAllItemsOnGrid(IGridView gridView)
        {
            var gg = gridView.Grid;
            var camera = Camera.main;
            var saves = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>();
            var db = ServiceLocator.Get<HeroesDatabase>();
            for (var y = 0; y < gg.GetLength(1); y++)
            {
                for (var x = 0; x < gg.GetLength(0); x++)
                {
                    var cell = (MergeCellView)gridView.GetCell(x,y);
                    if (cell.itemView == null)
                    {
                        uiGrid[x,y].gameObject.SetActive(false);
                        continue;
                    }
                    var ui = uiGrid[x, y];
                    ui.gameObject.SetActive(true);
                    // ui.transform.position = camera.WorldToScreenPoint(cell.LowerPoint.position);
                    ui.tracker.SetTarget(cell.LowerPoint);
                    ui.tracker.enabled = true;
                    var heroId = cell.itemView.itemData.core.id;
                    var data = saves.GetSave(heroId);
                    var info = db.GetHeroInfo(heroId);
                    if (data.isUnlocked == false)
                    {
                        ui.ShowNotOwned(info.viewInfo.name);
                        continue;
                    }
                    ui.ShowInfo(info.viewInfo.name, data.level, data.xp, data.xpForNext);
                }                
            }
        }

        public BarrackUnitInfo GetUIForCell(int x, int y)
        {
            return uiGrid[x, y];
        }

        public BarrackUnitInfo GetUI()
        {
            return (BarrackUnitInfo)_pool.GetOne();
        }
    }
}