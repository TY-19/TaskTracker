import { Component, OnInit } from '@angular/core';
import { Stage } from '../models/stage';
import { StageService } from './stage.service';
import { ActivatedRoute } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';
import { DisplayModes } from '../common/enums/display-modes';
import { sortStagesByPosition } from '../common/helpers/comparators';

@Component({
  selector: 'tt-stages',
  templateUrl: './stages.component.html',
  styleUrls: ['./stages.component.scss']
})
export class StagesComponent implements OnInit {

  boardId!: string;
  stages!: MatTableDataSource<Stage>;
  
  showPanel: boolean = false;
  mode: DisplayModes = DisplayModes.View;

  currentStageId: number = 0;
  highlightedRow: number | null = null;

  constructor(private stageService: StageService,
    private activatedRoute: ActivatedRoute) { 

  }

  ngOnInit(): void {
    this.boardId = this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.loadStages();
  }

  private loadStages(): void {
    this.stageService.getStages(this.boardId)
      .subscribe(result => {
        result.sort(sortStagesByPosition);
        this.stages = new MatTableDataSource(result);
      });
  }

  refreshStages(): void
  {
    this.loadStages();
    this.changeMode(DisplayModes.View);
  }

  setStageId(stageId: number): void {
    this.currentStageId = stageId;
  }

  moveStage(stageId: number, forward: boolean): void {
    this.stageService.moveStage(this.boardId, stageId, forward)
      .subscribe(() => this.loadStages());
  }

  deleteStage(stageId: number): void {
    this.stageService.deleteStage(this.boardId, stageId.toString())
      .subscribe(() => this.loadStages());
  }

  get isInCreateMode(): boolean {
    return this.mode === DisplayModes.Create;
  }

  get isInEditMode(): boolean {
    return this.mode === DisplayModes.Edit;
  }

  setModeToView(): void {
    this.changeMode(DisplayModes.View);
  }

  setModeToCreate(): void {
    this.changeMode(DisplayModes.Create);
  }

  setModeToEdit(): void {
    this.changeMode(DisplayModes.Edit);
  }

  private changeMode(mode: DisplayModes): void {
    this.mode = mode;
    this.showPanel = this.isInEditMode;
  }
  
  highlightRow(rowIndex: number): void {
    this.highlightedRow = rowIndex;
  }
}
