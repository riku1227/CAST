using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SCENE_EDIT;

namespace CAST.Loader
{
    public class CategoryPartsLoadere
    {
        public static int baseId = 1000;
        public static void LoadCategory()
        {
            var categoryList = EditCategoryPartsData.DataList;
            var enabledList = EditCategoryPartsData.EnabledList;

            AddEditCategoryPartsData(SceneEditCategoryWindow.CategoryType.DRESS_PARTS, "bra", "customview_icon_onepiece.tex", MPN.bra, ref categoryList, ref enabledList);
            AddEditCategoryPartsData(SceneEditCategoryWindow.CategoryType.DRESS_PARTS, "pants", "customview_icon_onepiece.tex", MPN.panz, ref categoryList, ref enabledList);
            AddEditCategoryPartsData(SceneEditCategoryWindow.CategoryType.DRESS_PARTS, "accanl", "customview_icon_onepiece.tex", MPN.accanl, ref categoryList, ref enabledList);
            AddEditCategoryPartsData(SceneEditCategoryWindow.CategoryType.DRESS_PARTS, "accvag", "customview_icon_onepiece.tex", MPN.accvag, ref categoryList, ref enabledList);
            AddEditCategoryPartsData(SceneEditCategoryWindow.CategoryType.DRESS_PARTS, "accxxx", "customview_icon_onepiece.tex", MPN.accxxx, ref categoryList, ref enabledList);
            AddEditCategoryPartsData(SceneEditCategoryWindow.CategoryType.BODY_PARTS, "body", "customview_icon_onepiece.tex", MPN.body, ref categoryList, ref enabledList);

            var editCategoryPartsDataType = typeof(EditCategoryPartsData);
            Util.setPrivateStaticField(editCategoryPartsDataType, "m_dataList", categoryList);
            Util.setPrivateStaticField(editCategoryPartsDataType, "m_enabledList", enabledList);
        }

        public static void AddEditCategoryPartsData(SceneEditCategoryWindow.CategoryType categoryType, String name, String iconFileName, MPN mpn,
            ref List<EditCategoryPartsData> categoryList,ref HashSet<int> enabledList)
        {
            var editCategoryPartsData = new EditCategoryPartsData();
            var editCategoryPartsDataType = typeof(EditCategoryPartsData);

            Util.invokePrivateSetter(editCategoryPartsDataType, editCategoryPartsData, "ID", baseId);
            Util.invokePrivateSetter(editCategoryPartsDataType, editCategoryPartsData, "Type", categoryType);
            Util.invokePrivateSetter(editCategoryPartsDataType, editCategoryPartsData, "Name", name);
            Util.invokePrivateSetter(editCategoryPartsDataType, editCategoryPartsData, "IconFileName", iconFileName);
            Util.invokePrivateSetter(editCategoryPartsDataType, editCategoryPartsData, "Mpn", mpn);

            categoryList.Add(editCategoryPartsData);
            enabledList.Add(baseId);

            baseId += 10;
        }
    }
}
