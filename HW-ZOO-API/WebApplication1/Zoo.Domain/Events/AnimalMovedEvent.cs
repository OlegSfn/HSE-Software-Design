using Zoo.Domain.ValueObjects;

namespace Zoo.Domain.Events
{
    public class AnimalMovedEvent
    {
        public Guid AnimalId { get; }
        public Name AnimalName { get; }
        public Guid? SourceEnclosureId { get; }
        public Guid TargetEnclosureId { get; }
        public DateTime Timestamp { get; }
        
        public AnimalMovedEvent(Guid animalId, Name animalName, Guid? sourceEnclosureId, Guid targetEnclosureId)
        {
            AnimalId = animalId;
            AnimalName = animalName;
            SourceEnclosureId = sourceEnclosureId;
            TargetEnclosureId = targetEnclosureId;
            Timestamp = DateTime.Now;
        }
    }
}