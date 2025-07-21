@echo off
REM Create backup directory if it doesn't exist
mkdir "Assets\LegacyScriptsBackup"

REM Move legacy gameplay scripts
move "Assets\Scripts\Gameplay\MatchDetector.cs" "Assets\LegacyScriptsBackup\"
move "Assets\Scripts\Gameplay\ScoreManager.cs" "Assets\LegacyScriptsBackup\"
move "Assets\Scripts\Gameplay\BlockSpawner.cs" "Assets\LegacyScriptsBackup\"

REM Move legacy utility/test scripts
move "Assets\Scripts\Utilities\FinalCompilationCheck.cs" "Assets\LegacyScriptsBackup\"
move "Assets\Scripts\Utilities\QuickTest.cs" "Assets\LegacyScriptsBackup\"

REM (Optional) Move any .meta files as well
move "Assets\Scripts\Gameplay\MatchDetector.cs.meta" "Assets\LegacyScriptsBackup\"
move "Assets\Scripts\Gameplay\ScoreManager.cs.meta" "Assets\LegacyScriptsBackup\"
move "Assets\Scripts\Gameplay\BlockSpawner.cs.meta" "Assets\LegacyScriptsBackup\"
move "Assets\Scripts\Utilities\FinalCompilationCheck.cs.meta" "Assets\LegacyScriptsBackup\"
move "Assets\Scripts\Utilities\QuickTest.cs.meta" "Assets\LegacyScriptsBackup\"

echo All legacy scripts moved to Assets\LegacyScriptsBackup
pause
