export class Purchase {
  purchaseId: number = 0;
  itemId: number = 0;
  itemName: string = '';
  amountPaid: number = 0;
  change: number = 0;
  purchaseDate: string = '';

  constructor(
    purchaseId: number,
    itemId: number,
    itemName: string,
    amountPaid: number,
    change: number,
    purchaseDate: string
  ) {
    this.purchaseId = purchaseId;
    this.itemId = itemId;
    this.amountPaid = amountPaid;
    this.itemName = itemName;
    this.change = change;
    this.purchaseDate = purchaseDate;
  }
}
