//using AutoMapper;
//using FluentValidation;
//using FluentValidation.Results;
//using MediatR;
//using WePlayRises.BuildingBlocks.Kernel.Extensions;
//using WePlayRises.BuildingBlocks.Kernel.Http.Response;
//using WePlayRises.BuildingBlocks.Kernel.Services;

//namespace WePlayRises.BuildingBlocks.Kernel.Features.Queries.GetById
//{
//    public class GetByIdQueryWithValidatorHandlerBase<TEntity, TDto, TKey> : IRequestHandler<GetByIdQueryWithValidatorBase<TDto, TKey>, ServiceResponse<TDto>>
//        where TEntity : class // TEntity debe ser un tipo de referencia
//        where TDto : class // TDto también debe ser un tipo de referencia
//        where TKey : IEquatable<TKey> // TKey debe implementar IEquatable<TKey>
//    {
//        private readonly IGetById<TEntity, TKey> _service;
//        private readonly IMapper _mapper;
//        private readonly IValidator<GetByIdQueryWithValidatorBase<TDto, TKey>> _validator;

//        public GetByIdQueryWithValidatorHandlerBase(IGetById<TEntity, TKey> service, IMapper mapper, IValidator<GetByIdQueryWithValidatorBase<TDto, TKey>> validator)
//        {
//            _service = service ?? throw new ArgumentNullException(nameof(service));
//            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
//            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
//        }

//        public async Task<ServiceResponse<TDto>> Handle(GetByIdQueryWithValidatorBase<TDto, TKey> request, CancellationToken cancellationToken)
//        {
//            if (request == null) throw new ArgumentNullException(nameof(request));

//            ValidationResult validationResult = await _validator.ValidateAsync(request);
//            if (!validationResult.IsValid)
//            {
//                return new ServiceResponse<TDto>
//                {
//                    Messages = validationResult.GetServiceResponseMessages()
//                };
//            }


//            //// Llama al servicio para obtener la entidad
//            //var entity = await _service.GetByIdAsync(request.Id, cancellationToken);
//            var entity = _service.GetById(request.Id);
//            if (entity == null)
//            {
//                return default!;
//            }

//            return new ServiceResponse<TDto>
//            {
//                Data = _mapper.Map<TDto>(entity)
//            };
//        }
//    }
//}
