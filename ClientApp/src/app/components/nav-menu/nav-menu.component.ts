import { Component, OnInit } from '@angular/core';
@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html'
})
export class NavMenuComponent implements OnInit {
  isTablePage: boolean = false;
  isSqlPage: boolean = false;

  ngOnInit(): void {
    var path = location.pathname.split('/')[1];
    this.isSqlPage = path === 'sql';
    this.isTablePage = path === 'tables' || path === '';
  }

  clickOnTable(){
    this.isTablePage = true;
    this.isSqlPage = false;
  }
  clickOnSql(){ 
    this.isTablePage = false;
    this.isSqlPage = true;
  }
}
