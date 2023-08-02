import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BoardService } from '../board.service';
import { FormControl, FormGroup,  Validators } from '@angular/forms';
import { Board } from 'src/app/models/board';
import { CustomValidators } from 'src/app/common/custom-validators';

@Component({
  selector: 'tt-board-edit',
  templateUrl: './board-edit.component.html',
  styleUrls: ['./board-edit.component.scss']
})
export class BoardEditComponent implements OnInit {

  board!: Board;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private boardService: BoardService) { 

    }

  form!: FormGroup;
  
  ngOnInit(): void {
    this.initiateForm();
    this.loadBoard();
  }

  private initiateForm()
  {
    this.form = new FormGroup({
      id: new FormControl(),
      name: new FormControl("", [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(50),
        CustomValidators.boardNameValidator()
      ])
    });
  }

  onSubmit(): void {
    if(this.form.valid)
    {
      let toUpdate: Board = this.board;
      toUpdate.name = this.form.controls['name'].value;
      this.updateBoard(toUpdate);
    } else {
      this.form.markAllAsTouched();
    }
  }

  private loadBoard() {
    let idParam = this.activatedRoute.snapshot.paramMap.get('id');
    
    this.boardService.getBoard(idParam!).subscribe({
      next: result => { 
        this.board = result;
        this.form.patchValue(this.board);
      },
      error: error => console.error(error),
    })
  }

  private updateBoard(board: Board) {
    let idParam = this.activatedRoute.snapshot.paramMap.get('id');
    if (!idParam || idParam != board.id.toString())
    {
      console.log("Id of the board doesn't match url")
      return;
    }

    this.boardService.updateBoard(idParam, board).subscribe(() => { 
        this.router.navigate(['/boards'])
          .catch(error => console.log(error))
    });
  }

}
