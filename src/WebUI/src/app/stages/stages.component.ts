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
  
  stages!: MatTableDataSource<Stage>;

  constructor(private stageService: StageService,
    private activatedRoute: ActivatedRoute) { 

  }

  ngOnInit(): void {
    this.getStages();
  }

  getStages() {
    let boardId = this.activatedRoute.snapshot.paramMap.get('boardId');
    this.stageService.getStages(boardId!)
      .subscribe(result => {
        result.sort((a, b) => a.position - b.position);
        this.stages = new MatTableDataSource(result);
      });
  }

  moveStage(stageId: number, forward: boolean) {
    let boardId = this.activatedRoute.snapshot.paramMap.get('boardId');
    this.stageService.moveStage(boardId!, stageId.toString(), forward)
      .subscribe( () => this.getStages() );
  }

}
