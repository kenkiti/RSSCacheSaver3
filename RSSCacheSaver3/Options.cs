﻿

namespace RSSCacheSaver3
{
    class Options
    {
        // 起動時に読み込む銘柄コード
        [CommandLine.Option('c')]
        public string Code { get; set; }

        // 自動開始
        [CommandLine.Option('a')]
        public bool AutoStart { get; set; }
    }

}
