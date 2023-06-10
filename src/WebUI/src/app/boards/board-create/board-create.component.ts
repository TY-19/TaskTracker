import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { BoardService } from '../board.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Board } from 'src/app/models/board';

@Component({
  selector: 'tt-board-create',
  templateUrl: './board-create.component.html',
  styleUrls: ['./board-create.component.scss']
})
export class BoardCreateComponent implements OnInit {

  board!: Board;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private boardService: BoardService) { 

    }

  form!: FormGroup;
  
  ngOnInit(): void {
    this.initiateForm();
  }

  private initiateForm()
  {
    this.form = new FormGroup({
      name: new FormControl("", [
        Validators.required,
        Validators.minLength(3),
        this.boardService.boardNameValidator()
      ])
    });
  }

  onSubmit(): void {
    if(this.form.valid)
    {
      let toCreate: Board = { id: 0, 
        name: this.form.controls['name'].value };
      this.createBoard(toCreate);
    }
  }

  private createBoard(board: Board) {
    this.boardService.createBoard(board).subscribe(() => { 
        this.router.navigate(['/boards'])
          .catch(error => console.log(error))
    });
  }

}
