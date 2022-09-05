namespace Microsoft.Win32
{
    using System;

    [Flags]
    public enum TVIF : uint
    {
        TVIF_CHILDREN = 0x40,
        TVIF_DI_SETITEM = 0x1000,
        TVIF_EXPANDEDIMAGE = 0x200,
        TVIF_HANDLE = 0x10,
        TVIF_IMAGE = 2,
        TVIF_INTEGRAL = 0x80,
        TVIF_PARAM = 4,
        TVIF_SELECTEDIMAGE = 0x20,
        TVIF_STATE = 8,
        TVIF_STATEEX = 0x100,
        TVIF_TEXT = 1
    }

    [Flags]
    public enum TVM : uint
    {
        TV_FIRST = 4352,

        TVSIL_NORMAL = 0,
        TVSIL_STATE = 2,

        TVM_SETIMAGELIST = TV_FIRST + 9,
        TVM_GETNEXTITEM = TV_FIRST + 10,
        TVM_GETITEM = TV_FIRST + 12,
        TVM_SETITEM = TV_FIRST + 13,
        TVM_HITTEST = TV_FIRST + 17,

        TVIF_HANDLE = 16,
        TVIF_STATE = 8,

        TVIS_STATEIMAGEMASK = 61440,

        TVGN_ROOT = 0,

        //TVHITTESTINFO.flags flags
        TVHT_ONITEMSTATEICON = 64,
    }
}