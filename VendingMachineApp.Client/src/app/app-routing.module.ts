import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { VendingMachineComponent } from './components/vending-machine/vending-machine.component';
import { ReportComponent } from './components/report/report.component';

const routes: Routes = [
  { path: '', component: VendingMachineComponent },
  { path: 'vending-machine', component: VendingMachineComponent },
  { path: 'report', component: ReportComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
