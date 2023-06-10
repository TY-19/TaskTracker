import { Component, OnInit } from '@angular/core';
import { BoardService } from '../board.service';
import { Board } from 'src/app/models/board';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'tt-board-details',
  templateUrl: './board-details.component.html',
  styleUrls: ['./board-details.component.scss']
})
export class BoardDetailsComponent implements OnInit {

  board! : Board;
  
  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private boardService: BoardService) { 

  }

  ngOnInit(): void {
    this.getBoard();
  }

  getBoard() {
    let idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.boardService.getBoard(idParam!)
      .subscribe(result => this.board = result);
  }
}
