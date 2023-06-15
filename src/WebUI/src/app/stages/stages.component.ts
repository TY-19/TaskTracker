import { Component, OnInit } from '@angular/core';
import { Stage } from '../models/stage';
import { StageService } from './stage.service';
import { ActivatedRoute } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'tt-stages',
  templateUrl: './stages.component.html',
  styleUrls: ['./stages.component.scss']
})
export class StagesComponent implements OnInit {

  boardId: string = "";
  stages!: MatTableDataSource<Stage>;
  
  showPanel: boolean = false;
  mode: string = "view";
  currentStageId: number = 0;
  highlightedRow: number | null = null;

  constructor(private stageService: StageService,
    private activatedRoute: ActivatedRoute) { 

  }

  ngOnInit(): void {
    this.boardId = this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.getStages();
  }

  getStages() {
    this.stageService.getStages(this.boardId)
      .subscribe(result => {
        result.sort((a, b) => a.position - b.position);
        this.stages = new MatTableDataSource(result);
      });
  }

  refreshStages()
  {
    this.getStages();
    this.changeMode('view');
  }

  setStageId(stageId: number) {
    this.currentStageId = stageId;
  }

  moveStage(stageId: number, forward: boolean) {
    this.stageService.moveStage(this.boardId, stageId.toString(), forward)
      .subscribe( () => this.getStages() );
  }

  deleteStage(stageId: number) {
    this.stageService.deleteStage(this.boardId, stageId.toString())
      .subscribe(() => this.getStages());
  }

  changeMode(mode: string) {
    this.mode = mode;
    if (mode === 'create' || mode === 'edit')    
        this.showPanel = true;
    else
        this.showPanel = false;
  }
  
  highlightRow(rowIndex: number) {
    this.highlightedRow = rowIndex;
  }

}
