package uk.ac.kingston.ci7250.ims.service;

import org.springframework.stereotype.Service;
import uk.ac.kingston.ci7250.ims.domain.ImageRecord;
import uk.ac.kingston.ci7250.ims.domain.Modality;
import uk.ac.kingston.ci7250.ims.repo.ImageRepository;
import uk.ac.kingston.ci7250.ims.repo.PatientRepository;

import java.time.Instant;
import java.util.List;

@Service
public class ImagingService {
    private final PatientRepository patientRepository;
    private final ImageRepository imageRepository;

    public ImagingService(PatientRepository patientRepository, ImageRepository imageRepository) {
        this.patientRepository = patientRepository;
        this.imageRepository = imageRepository;
    }

    public ImageRecord upload(long patientId, Modality modality, String diseaseTag) {
        if (modality == null) {
            throw new IllegalArgumentException("Modality is required");
        }
        if (diseaseTag == null || diseaseTag.isBlank()) {
            throw new IllegalArgumentException("Disease tag is required");
        }

        patientRepository.require(patientId);
        long id = imageRepository.nextId();
        ImageRecord rec = new ImageRecord(id, patientId, modality, diseaseTag, Instant.now());
        imageRepository.create(rec);
        return rec;
    }

    public List<ImageRecord> listForPatient(long patientId) {
        patientRepository.require(patientId);
        return imageRepository.findByPatientIdSorted(patientId);
    }

    public void delete(long imageId) {
        imageRepository.delete(imageId);
    }
}
