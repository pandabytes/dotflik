/// <reference types="jquery" />
/// <reference types="bootstrap" />

/**
  * Hide a modal with id `modalId` asynchronously.
  *
  * @remarks
  * This function will wait for the modal to be hidden before it ends.
  *
  * @param modalId - The id of a modal dialog
  */
export async function hideModal(modalId: string): Promise<void> {
  let modalHidden: boolean = false;
  const eventName: string = "hidden.bs.modal";

  const hiddenHandler = function () {
    modalHidden = true;
  };

  $(modalId).on(eventName, hiddenHandler);

  $(modalId).modal("hide");
  while (!modalHidden) {
    await sleep(100);
  }

  $(modalId).off(eventName, hiddenHandler);
}

/**
  * Show a modal with id `modalId` asynchronously.
  * 
  * @remarks
  * This function will wait for the modal to be shown before it ends.
  *
  * @param modalId - The id of a modal dialog
  */
export async function showModal(modalId: string): Promise<void> {
  let modalShown: boolean = false;
  const eventName: string = "shown.bs.modal";

  const shownHandler = function () {
    modalShown = true;
  };

  $(modalId).on(eventName, shownHandler);

  $(modalId).modal("show");
  while (!modalShown) {
    await sleep(100);
  }

  $(modalId).off(eventName, shownHandler);
}

/**
  * Sleep for `ms` milliseconds.
  * @param ms - The number of milliseconds to sleep
  */
async function sleep(ms: number) {
  return new Promise(resolve => setTimeout(resolve, ms));
}
