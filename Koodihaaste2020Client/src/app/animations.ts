export const smoothHeight = trigger('grow', [
  transition('void <=> *', []),
  transition('* <=> *', [style({ width: '{{startHeight}}px', opacity: 0 }), animate('.5s ease')], {
    params: { startHeight: 0 }
  })
]);