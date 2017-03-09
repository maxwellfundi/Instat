﻿' Instat-R
' Copyright (C) 2015
'
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License k
' along with this program.  If not, see <http://www.gnu.org/licenses/>.
Imports instat.Translations

Public Class dlgScatterPlot
    Private clsOverallFunction As New RFunction
    Private clsRGeomScatterplotFunction As New RFunction
    Private clsRaesFunction As New RFunction
    '    Private clsOperation As New ROperator
    Private bFirstLoad As Boolean = True
    Private bReset As Boolean = True
    '    Private bResetSubdialog As Boolean = False

    Private Sub dlgScatterPlot_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        autoTranslate(Me)
        If bFirstLoad Then
            InitialiseDialog()
            bFirstLoad = False
        End If
        If bReset Then
            SetDefaults()
        End If
        SetRCodeForControls(bReset)
        bReset = False
        TestOkEnabled()
    End Sub

    'TODO: when the main receiver is a ucrCore, then I can continue work. Possibly in the dialog
    '(parameternames stay the same, but = different things)

    '# dlgScatterPlot code generated by the dialog Scatter Plot
    'Blocking_temp <- InstatDataObject$get_data_frame(data_name="Blocking")
    'Scatter2 <- ggplot(data=Blocking_temp, mapping=aes(colour=x2, y=Trt, x=x1)) + geom_point()
    'InstatDataObject$add_graph(data_name="Blocking", graph=Scatter2, graph_name="Scatter2")
    'InstatDataObject$get_graphs(graph_name = "Scatter2", data_name = "Blocking")

    'or for several checked:
    'Blocking_temp <- InstatDataObject$get_data_frame(id.vars=c("Trt","x1"), measure.vars=c("Yield","x1","x2"),
    'data_name="Blocking", stack_data=TRUE)
    'Scatter3 <- ggplot(mapping=aes(x=x1, y=value, colour=variable), data=Blocking_temp) + geom_point()
    'InstatDataObject$add_graph(data_name = "Blocking", graph = Scatter3, graph_name = "Scatter3")
    'InstatDataObject$get_graphs(graph_name = "Scatter3", data_name = "Blocking")

    Private Sub InitialiseDialog()
        ucrBase.clsRsyntax.bExcludeAssignedFunctionOutput = False
        ucrBase.clsRsyntax.iCallType = 3
        ucrBase.iHelpTopicID = 433

        'ucrSelectorForScatter
        ucrSelectorForScatter.SetParameter(New RParameter("data", 0))
        ucrSelectorForScatter.SetParameterIsString()

        'ucrMainReceiver
        ' set parameter? canne if not inherits ucrCore
        ucrVariablesAsFactorForScatter.SetFactorReceiver(ucrFactorOptionalReceiver)
        ucrVariablesAsFactorForScatter.SetSelector(ucrSelectorForScatter)
        ucrVariablesAsFactorForScatter.SetIncludedDataType({"factor", "numeric"})

        'ucrReceiverX
        ucrReceiverX.SetParameter(New RParameter("x",)) ' only x = this if it is full, otherwise x = ""
        ucrReceiverX.Selector = ucrSelectorForScatter
        ucrReceiverX.SetIncludedDataTypes({"factor", "numeric"})
        ucrReceiverX.SetParameterIsString()
        ucrReceiverX.GetVariableNames(False)

        'ucrReceiverOptional
        ucrFactorOptionalReceiver.SetParameter(New RParameter("colour",)) 'clsaes, only x = this if it is full, otherwise remove param.
        ucrFactorOptionalReceiver.SetParameterIsString()
        ucrFactorOptionalReceiver.GetVariableNames(False)
        ucrFactorOptionalReceiver.Selector = ucrSelectorForScatter
        ucrFactorOptionalReceiver.SetIncludedDataTypes({"factor", "numeric"})

        'ucrSave
        ucrSaveGraph.SetPrefix("Scatter")
        ucrSaveGraph.SetSaveTypeAsGraph()
        ucrSaveGraph.SetDataFrameSelector(ucrSelectorForScatter.ucrAvailableDataFrames)
        ucrSaveGraph.SetCheckBoxText("Save Graph")
        ucrSaveGraph.SetIsComboBox()
        ucrSaveGraph.SetAssignToIfUncheckedValue("last_graph")

        '        sdgPlots.SetRSyntax(ucrBase.clsRsyntax)
    End Sub

    Private Sub SetDefaults()
        clsRaesFunction = New RFunction
        clsOverallFunction = New RFunction
        clsRGeomScatterplotFunction = New RFunction
        '        clsOperation = New ROperator

        ucrSaveGraph.Reset()
        ucrSelectorForScatter.Reset()
        ucrVariablesAsFactorForScatter.ResetControl()
        '        sdgPlots.Reset()

        'SetDefaults
        clsRaesFunction.ClearParameters()
        clsRGeomScatterplotFunction.ClearParameters()

        ' Default R
        clsOverallFunction.SetRCommand("ggplot")
        clsOverallFunction.AddParameter("data", clsRFunctionParameter:=ucrSelectorForScatter.ucrAvailableDataFrames.clsCurrDataFrame)
        ' the above line we usually wouldn't have, but if I don't put it here Blocking_temp <- InstatDataObject$get_data_frame(data_name="Blocking") doesn't run?

        'aes(colour = "", y = "", x = "")
        clsRaesFunction.SetRCommand("aes")
        clsRaesFunction.AddParameter("y", Chr(34) & Chr(34)) ' y is an empty string if it has nothing in it
        clsRaesFunction.AddParameter("x", Chr(34) & Chr(34)) ' x is an empty string if it has nothing in it

        'mapping = aes (colour = ...)
        clsOverallFunction.AddParameter("mapping", clsRFunctionParameter:=clsRaesFunction)

        'clsRGeom
        clsRGeomScatterplotFunction.SetRCommand("geom_point")

        ucrBase.clsRsyntax.SetBaseRFunction(clsOverallFunction)

        'Operations Set
        ucrBase.clsRsyntax.SetOperation("+")
        ucrBase.clsRsyntax.SetOperatorParameter(0, clsRFunc:=clsOverallFunction)
        ucrBase.clsRsyntax.SetOperatorParameter(1, clsRFunc:=clsRGeomScatterplotFunction)

        ucrBase.clsRsyntax.SetAssignTo("last_graph", strTempDataframe:=ucrSelectorForScatter.ucrAvailableDataFrames.cboAvailableDataFrames.Text, strTempGraph:="last_graph")

        ucrBase.clsRsyntax.SetBaseROperator(ucrBase.clsRsyntax.clsBaseOperator)

        '        bResetSubdialog = True
    End Sub

    Private Sub TestOkEnabled()
        'tests when okay Is enable. Both x and y aesthetics are mandatory but can be set to x="" or(exclusive) y="" in case the other one is filled. 
        If (Not ucrReceiverX.IsEmpty() OrElse Not ucrVariablesAsFactorForScatter.IsEmpty) AndAlso ucrSaveGraph.IsComplete Then
            ucrBase.OKEnabled(True)
        Else
            ucrBase.OKEnabled(False)
        End If
    End Sub

    Private Sub SetRCodeForControls(bReset As Boolean)
        ucrSaveGraph.SetRCode(clsOverallFunction, bReset)
        ucrSelectorForScatter.SetRCode(clsOverallFunction, bReset)

        ucrReceiverX.SetRCode(clsRaesFunction, bReset)
        ucrFactorOptionalReceiver.SetRCode(clsRaesFunction, bReset)
        '        ucrVariablesAsFactorForScatter.SetRCode(clsRaesFunction, bReset)
    End Sub

    Private Sub ucrBase_ClickReset(sender As Object, e As EventArgs) Handles ucrBase.ClickReset
        SetDefaults()
        SetRCodeForControls(True)
        TestOkEnabled()
    End Sub

    Private Sub cmdOptions_Click(sender As Object, e As EventArgs) Handles cmdOptions.Click
        '        sdgPlots.SetDataFrame(strNewDataFrame:=ucrSelectorForScatter.ucrAvailableDataFrames.cboAvailableDataFrames.Text)
        '        sdgPlots.ShowDialog()
    End Sub

    'this needs to be controlcontentschanged once it inherits ucrCore
    ' currently this only runs when I change the ucrVariablesAsFactor receiver, what if I open the dialog and don't put anything in at all? Then we don't get y = "" as we want
    Private Sub ucrVariablesAsFactor_SelectionChanged() Handles ucrVariablesAsFactorForScatter.SelectionChanged
        clsRaesFunction.RemoveParameterByName("y")
        If Not ucrVariablesAsFactorForScatter.IsEmpty Then
            clsRaesFunction.AddParameter("y", ucrVariablesAsFactorForScatter.GetVariableNames(False))
        Else
            clsRaesFunction.AddParameter("y", Chr(34) & Chr(34))
        End If
    End Sub

    Private Sub cmdScatterPlotOptions_Click(sender As Object, e As EventArgs) Handles cmdScatterPlotOptions.Click
        ''SetupLayer sends the components storing the plot info (clsRaesFunction, clsRggplotFunction, ...) of dlgScatteredPlot through to sdgLayerOptions where these will be edited.
        ''        sdgLayerOptions.SetupLayer(clsTempGgPlot:=clsRggplotFunction, clsTempGeomFunc:=clsRgeom_scatterplotFunction, clsTempAesFunc:=clsRaesFunction, bFixAes:=True, bFixGeom:=True, strDataframe:=ucrSelectorForScatter.ucrAvailableDataFrames.cboAvailableDataFrames.Text, bApplyAesGlobally:=True, bIgnoreGlobalAes:=False)
        ''        sdgLayerOptions.ShowDialog()
        ''Coming from the sdgLayerOptions, clsRaesFunction and others has been modified. One then needs to display these modifications on the dlgScatteredPlot.

        ''The aesthetics parameters on the main dialog are repopulated as required. 
        'For Each clsParam In clsRaesFunction.clsParameters
        '    If clsParam.strArgumentName = "x" Then
        '        If clsParam.strArgumentValue = Chr(34) & Chr(34) Then
        '            ucrReceiverX.Clear()
        '        Else
        '            ucrReceiverX.Add(clsParam.strArgumentValue)
        '        End If
        '        'In the y case, the vlue stored in the clsReasFunction in the multiplevariables case is "value", however that one shouldn't be written in the multiple variables receiver (otherwise it would stack all variables and the stack ("value") itself!).
        '        'Warning: what if someone used the name value for one of it's variables independently from the multiple variables method ? Here if the receiver is actually in single mode, the variable "value" will still be given back, which throws the problem back to the creation of "value" in the multiple receiver case.
        '    ElseIf clsParam.strArgumentName = "y" AndAlso (clsParam.strArgumentValue <> "value" OrElse ucrVariablesAsFactorForScatter.bSingleVariable) Then
        '        'Still might be in the case of bSingleVariable with mapping y="".
        '        If clsParam.strArgumentValue = (Chr(34) & Chr(34)) Then
        '            ucrVariablesAsFactorForScatter.Clear()
        '        Else ucrVariablesAsFactorForScatter.Add(clsParam.strArgumentValue)
        '        End If
        '    ElseIf clsParam.strArgumentName = "colour" Then
        '        ucrFactorOptionalReceiver.Add(clsParam.strArgumentValue)
        '    End If
        'Next
    End Sub

    Private Sub ucrControls_ControlContentsChanged(ucrChangedControl As ucrCore) Handles ucrSaveGraph.ControlContentsChanged, ucrReceiverX.ControlContentsChanged ', ucrVariablesAsFactorForScatter.ControlContentsChanged
        TestOkEnabled()
    End Sub

    ' currently this only runs when I change the ucrReceiverX, what if I open the dialog and don't put anything in at all? Then we don't get x = "" as we want
    Private Sub ucrReceiverX_ControlValueChanged(ucrChangedControl As ucrCore) Handles ucrReceiverX.ControlValueChanged
        clsRaesFunction.RemoveParameterByName("x")
        If Not ucrReceiverX.IsEmpty Then
            clsRaesFunction.AddParameter("x", ucrReceiverX.GetVariableNames(False))
        Else
            clsRaesFunction.AddParameter("x", Chr(34) & Chr(34))
        End If
    End Sub
End Class