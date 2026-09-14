
[Code]
// 全局变量，用于记录用户是否选择删除配置数据
var
  ShouldDeleteAppData: Boolean;
  
  function PromptUninstallOptions(): Boolean;
var
  CustomForm: TForm;
  ProcessRunning: Boolean;
  ResultCode: Integer;
  InfoLabel: TLabel;
  KeepDataCheck: TNewCheckBox;
  BtnUninstall, BtnCancel: TNewButton;
  TempLabel: TLabel;
begin
	// MsgBox('Current Language: ' + ActiveLanguage + #13#10 + 'Raw CM: ' + ExpandConstant('{cm:UninstallWizardTitle}'), mbInformation, MB_OK);
  Result := False;
  ShouldDeleteAppData := False;
  // 检测进程是否运行
  ProcessRunning := False;
  Exec(ExpandConstant('{cmd}'), '/c tasklist | find /I "{#MyAppExeName}"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  if ResultCode = 0 then ProcessRunning := True;
  CustomForm := TForm.Create(nil);
  try
    CustomForm.ClientWidth := 500;
    CustomForm.ClientHeight := 270;
    CustomForm.Caption := ExpandConstant('{cm:UninstallWizardTitle}');
    CustomForm.Position := poScreenCenter;
    CustomForm.BorderStyle := bsDialog;
    InfoLabel := TLabel.Create(CustomForm);
    InfoLabel.Parent := CustomForm;
    InfoLabel.Left := 25;
    InfoLabel.Top := 20;
    InfoLabel.AutoSize := False;
    InfoLabel.Width := 450;
    InfoLabel.Height := 145;
    InfoLabel.WordWrap := True;
    if ProcessRunning then
      InfoLabel.Caption := ExpandConstant('{cm:UninstallProcessRunning}')
    else
      InfoLabel.Caption := ExpandConstant('{cm:UninstallPreparing}');
    // 复选框
    KeepDataCheck := TNewCheckBox.Create(CustomForm);
    KeepDataCheck.Parent := CustomForm;
    KeepDataCheck.Left := 25;
    KeepDataCheck.Top := 175;
    KeepDataCheck.Width := 450;
    KeepDataCheck.Caption := ExpandConstant('{cm:KeepUserData}');
    KeepDataCheck.Checked := True;

    // 按钮
    // --- 先创建按钮并赋值文字 ---
    
    BtnUninstall := TNewButton.Create(CustomForm);
    BtnUninstall.Parent := CustomForm;
    BtnUninstall.Caption := ExpandConstant('{cm:ContinueUninstall}');
    BtnUninstall.ModalResult := mrOk;
    BtnUninstall.Top := CustomForm.ClientHeight - 45;
    
    BtnCancel := TNewButton.Create(CustomForm);
    BtnCancel.Parent := CustomForm;
    BtnCancel.Caption := ExpandConstant('{cm:UninstallCancel}');
    BtnCancel.ModalResult := mrCancel;
    BtnCancel.Top := CustomForm.ClientHeight - 45;

    // --- 创建隐藏的 Label 作为尺子 ---
    TempLabel := TLabel.Create(CustomForm);
    TempLabel.Parent := CustomForm;
    TempLabel.AutoSize := True;
    TempLabel.Visible := False; // 必须隐藏，不让用户看到

    // --- 动态计算宽度 ---
    
    // 测算取消按钮
    TempLabel.Caption := BtnCancel.Caption;
    BtnCancel.Width := TempLabel.Width + 30; // 标签自动撑开后的宽度 + 左右各15px内边距
    if BtnCancel.Width < 85 then BtnCancel.Width := 85; // 保底宽度
    
    // 测算卸载按钮
    TempLabel.Caption := BtnUninstall.Caption;
    BtnUninstall.Width := TempLabel.Width + 30; // 标签自动撑开后的宽度 + 左右各15px内边距
    if BtnUninstall.Width < 90 then BtnUninstall.Width := 90; // 保底宽度
    
    // --- 动态计算位置 (从右向左排版) ---
    
    // 最右侧留白 10 像素
    BtnCancel.Left := CustomForm.ClientWidth - BtnCancel.Width - 10;
    
    // 卸载按钮放在取消按钮的左边，中间留白 10 像素
    BtnUninstall.Left := BtnCancel.Left - BtnUninstall.Width - 10;
	
	
    if CustomForm.ShowModal() = mrOk then
    begin
      if ProcessRunning then
      begin
        Exec(ExpandConstant('{sys}\taskkill.exe'), '/F /IM {#MyAppExeName} /T', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
        Sleep(500);
      end;
      ShouldDeleteAppData := not KeepDataCheck.Checked;
      Result := True;
    end;
  finally
    CustomForm.Free;
  end;
end;

// 关键修改：把自定义窗口放在这里
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usUninstall then
  begin
    // 显示自定义卸载选项窗口
    if not PromptUninstallOptions() then
    begin
      // 用户点击了取消，中止卸载
      Abort;
    end;
  end
  else if CurUninstallStep = usPostUninstall then
  begin
    // 卸载完成后清理配置数据
    if ShouldDeleteAppData then
    begin
      DelTree(ExpandConstant('{#MyAppDataDir}'), True, True, True);
    end;
  end;
end;