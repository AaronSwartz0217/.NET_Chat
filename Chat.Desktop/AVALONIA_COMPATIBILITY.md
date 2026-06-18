# Avalonia UI 兼容性注意事项

> 本项目使用 Avalonia 11 框架，以下列出与 WPF 的差异及不支持的特性。
> 开发时请避免使用这些写法，否则会导致编译错误或运行时崩溃。

---

## XAML 绑定差异

### 1. `Classes` 属性不支持 Binding
```xml
<!-- ❌ 错误：Classes 不接受 CompiledBinding -->
<Button Classes="{Binding SomeValue, Converter=...}" />

<!-- ✅ 正确：用 IsVisible 双按钮切换，或 Background 区分 -->
<Button Background="{Binding IsSelected, Converter={x:Static local:SelectedBgConverter.Instance}}" />
```

### 2. `MultiBinding` 中不能直接写值
```xml
<!-- ❌ 错误：<Binding>0</Binding> 在 Avalonia 中不被识别 -->
<MultiBinding Converter="{x:Static local:SomeConverter.Instance}">
    <Binding Path="CurrentIndex"/>
    <Binding>0</Binding>   <!-- 编译错误 -->
</MultiBinding>

<!-- ✅ 正确：用 ConverterParameter 传参，或在 ViewModel 层处理逻辑 -->
<Binding Path="CurrentIndex" Converter="{x:Static local:IntEqualsConverter.Instance}"
         ConverterParameter="0"/>
```

### 3. `ItemsControl` 没有 `HorizontalContentAlignment`
```xml
<!-- ❌ 错误 -->
<ItemsControl HorizontalContentAlignment="Stretch">

<!-- ✅ 正确：用全局 Style 选择器（但要注意不要影响内部容器） -->
<!-- 或改用 ListBox 替代 ItemsControl -->
```

### 4. `ItemsControl` 没有 `ItemContainerStyle`
```xml
<!-- ❌ 错误 -->
<ItemsControl>
    <ItemsControl.ItemContainerStyle>
        <Style Selector="ContentPresenter">
            <Setter Property="HorizontalAlignment" Value="Stretch"/>
        </Style>
    </ItemsControl.ItemContainerStyle>
</ItemsControl>

<!-- ✅ 正确：在 UserControl.Styles 或 Grid.Styles 中用选择器 -->
<UserControl.Styles>
    <Style Selector="ItemsControl > ContentPresenter">
        <Setter Property="HorizontalAlignment" Value="Stretch"/>
    </Style>
</UserControl.Styles>
```

### 5. `$parent[Type]` 多层嵌套不可靠
```xml
<!-- ⚠️ 谨慎使用：多层 $parent 在 Avalonia 中经常解析失败 -->
Command="{Binding $parent[StackPanel].$parent[StackPanel].DataContext.SomeCommand}"

<!-- ✅ 正确：避免改变 DataContext，保持 DataContext 为 ViewModel -->
<!-- 用 {Binding SelectedPost.Title} 代替 DataContext="{Binding SelectedPost}" -->
```

### 6. `Interaction.Behaviors` / `EventBehavior` 不存在
```xml
<!-- ❌ 错误：WPF 的 Interaction 行为在 Avalonia 中不存在 -->
xmlns:i="https://github.com/avaloniaui"
<i:Interaction.Behaviors>
    <i:EventBehavior Event="Click" Command="{Binding ...}"/>
</i:Interaction.Behaviors>

<!-- ✅ 正确：用 Button.Command 直接绑定，或用透明 Button 包裹其他控件 -->
<Button Command="{Binding ClickCommand}" Classes="Transparent">
    <!-- 内部放任何内容 -->
</Button>
```

### 7. `SelectableTextBlock` 不存在
```xml
<!-- ❌ 错误：Avalonia 没有内置 SelectableTextBlock -->
<SelectableTextBlock Text="{Binding Content}"/>

<!-- ✅ 正确：用普通 TextBlock 替代 -->
<TextBlock Text="{Binding Content}"/>
```

### 8. `StackPanel` / `DockPanel` 没有 `Padding`
```xml
<!-- ❌ 错误：这些布局容器没有 Padding 属性 -->
<StackPanel Padding="12,8"/>

<!-- ✅ 正确：用 Margin 替代 Padding -->
<StackPanel Margin="12,8"/>
```

### 9. 全局 `ContentPresenter` Style 影响范围过大
```xml
<!-- ⚠️ 危险：Selector="ContentPresenter" 会影响所有 ContentPresenter -->
<Style Selector="ContentPresenter">
    <Setter Property="HorizontalAlignment" Value="Stretch"/>
</Style>

<!-- ✅ 正确：尽量不用全局样式，改用具体控件的属性设置 -->
```

---

## C# / API 差异

### 10. `IMultiValueConverter.Convert` 签名不同
```csharp
// WPF 签名（❌ Avalonia 不支持）
object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

// Avalonia 签名（✅ 必须用这个）
object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture);
```

### 11. 命名空间约定
- **WPF**: 控件通常放在 `Views` 子命名空间
- **Avalonia**: 部分控件可能直接在根命名空间（如 `ChatView` 是 `Chat.Desktop.ChatView`）

```xml
<!-- 注意区分 -->
xmlns:local="using:Chat.Desktop"           <!-- 根命名空间的控件 -->
xmlns:views="using:Chat.Desktop.Views"     <!-- Views 子命名空间的控件 -->
```

---

## JSON 序列化注意事项

### 12. camelCase 字段映射
后端 API 返回的 JSON 字段是 **camelCase**（如 `userName`, `createdTime`），但 C# 模型属性是 **PascalCase**（如 `UserName`, `CreatedTime`）。

默认 `JsonSerializer` 不会自动转换大小写，必须显式配置：

```csharp
private static readonly JsonSerializerOptions _jsonOptions = new()
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  // 关键！
    WriteIndented = false
};

// 使用
var post = JsonSerializer.Deserialize<PostModel>(json, _jsonOptions);
```

---

## 快速排查清单

遇到以下错误时，对照此表排查：

| 错误信息 | 可能原因 | 解决方案 |
|---------|---------|---------|
| `MarkupExtension expressions must end with a '}'` | 绑定表达式语法错误 | 检查 `\|\|`、`$parent` 等是否被 Avalonia 支持 |
| `Unable to resolve type xxx from namespace` | 类型或命名空间不存在 | 确认类是否在该命名空间下 |
| `Unable to find suitable setter for property Classes` | Classes 不支持 Binding | 改用 Background/IsVisible 切换 |
| `No Content property found for type CompiledBindingExtension` | MultiBinding 中写了 `<Binding>值</Binding>` | 改用 ConverterParameter |
| `The method or operation is not implemented` | 转换器缺少方法实现 | 补全接口的所有方法 |
| `System.NotImplementedException` 运行时崩溃 | 使用了不存在的控件（如 SelectableTextBlock） | 替换为等效的标准控件 |

---
*最后更新：2026-06-18*
