[1;33mdiff --git a/solutions/24/AoC-Day24/MainWindow.xaml b/solutions/24/AoC-Day24/MainWindow.xaml[m
[1;33mindex bc2b633..f7f7e93 100644[m
[1;33m--- a/solutions/24/AoC-Day24/MainWindow.xaml[m
[1;33m+++ b/solutions/24/AoC-Day24/MainWindow.xaml[m
[1;35m@@ -22,7 +22,8 @@[m
     </Window.Resources>    [m
     <ScrollViewer>[m
         <Grid>[m
[1;31m-            <DockPanel Name="dockPanel" Height="30" VerticalAlignment="Top" LastChildFill="False" HorizontalAlignment="Right" Margin="0,10,10,0" Panel.ZIndex="10000">[m
[1;32m+[m[1;32m            <DockPanel Name="dockPanel" Height="30" VerticalAlignment="Top" LastChildFill="False" HorizontalAlignment="Left" Margin="0,10,10,0" Panel.ZIndex="10000">[m
[1;32m+[m[1;32m                <Label Name="answerLabel" Foreground="Silver" Margin="10,0,10,0"/>[m
                 <Button Name="buttonSimulate" Click="Simulate" Template="{DynamicResource AoCButton}" Margin="10,0,0,0" Width="100">[Simulate]</Button>[m
                 <Button Name="buttonRepair" Click="Repair" Template="{DynamicResource AoCButton}" Margin="10,0,0,0" Width="100">[Repair]</Button>[m
             </DockPanel>[m
[1;33mdiff --git a/solutions/24/AoC-Day24/MainWindow.xaml.cs b/solutions/24/AoC-Day24/MainWindow.xaml.cs[m
[1;33mindex d81b921..7230020 100644[m
[1;33m--- a/solutions/24/AoC-Day24/MainWindow.xaml.cs[m
[1;33m+++ b/solutions/24/AoC-Day24/MainWindow.xaml.cs[m
[1;35m@@ -71,6 +71,15 @@[m [mnamespace AoC_Day24[m
             circuit.Import();[m
 [m
             DrawCircuit();[m
[1;32m+[m
[1;32m+[m[1;32m            var answer = string.Empty;[m
[1;32m+[m[1;32m            foreach (var wire in circuit.wires.Values)[m
[1;32m+[m[1;32m                if (wire.suspicious)[m
[1;32m+[m[1;32m                    answer += $"{wire.name},";[m
[1;32m+[m
[1;32m+[m[1;32m            answerLabel.FontFamily = consolasFamily;[m
[1;32m+[m[1;32m            answerLabel.FontSize = cellHeight * 0.4;[m
[1;32m+[m[1;32m            answerLabel.Content = $"Puzzle answer: {answer[..^1]}";[m
         }[m
 [m
         private void StyleButton(Button button)[m
[1;35m@@ -94,16 +103,6 @@[m [mnamespace AoC_Day24[m
                 DrawConnection(gate, gate.output, $"{gate.op}{gate.output.name}", gate.suspicious);[m
             }[m
 [m
[1;31m-            var answer = string.Empty;[m
[1;31m-            foreach (var wire in circuit.wires.Values)[m
[1;31m-                if (wire.suspicious)[m
[1;31m-                    answer += $"{wire.name},";[m
[1;31m-[m
[1;31m-            if (answer.Length > 0)[m
[1;31m-                answer = answer[..^1];[m
[1;31m-[m
[1;31m-            DrawAnswer($"Puzzle answer: {answer}");[m
[1;31m-[m
             storyboard = new Storyboard();[m
 [m
             foreach (var connection in connections)[m
[1;35m@@ -276,26 +275,6 @@[m [mnamespace AoC_Day24[m
             }[m
         }[m
 [m
[1;31m-        private void DrawAnswer(string answer)[m
[1;31m-        {[m
[1;31m-            var label = new TextBox[m
[1;31m-            {[m
[1;31m-                Background = Brushes.Transparent,[m
[1;31m-                BorderThickness = new Thickness(0),[m
[1;31m-                FontFamily = consolasFamily,[m
[1;31m-                FontSize = cellHeight * 0.4,[m
[1;31m-                FontWeight = FontWeights.Light,[m
[1;31m-                Foreground = Brushes.Silver,[m
[1;31m-                Text = answer,[m
[1;31m-                Height = cellHeight,[m
[1;31m-                TextAlignment = TextAlignment.Left,[m
[1;31m-                VerticalContentAlignment = VerticalAlignment.Center[m
[1;31m-            };[m
[1;31m-            Canvas.SetLeft(label, spacing);[m
[1;31m-            Canvas.SetTop(label, spacing);[m
[1;31m-            canvas.Children.Add(label);[m
[1;31m-        }[m
[1;31m-[m
         private void DrawWire(Wire wire)[m
         {[m
             wire.uiElement = DrawWire(wire.name, wire.position.x, wire.position.y, wire.position.offset, wire.suspicious);[m
