package uk.ac.kingston.ci7250.ims.service;

import org.springframework.stereotype.Service;
import uk.ac.kingston.ci7250.ims.domain.Patient;
import uk.ac.kingston.ci7250.ims.repo.PatientRepository;

import java.util.List;

@Service
public class PatientService {
    private final PatientRepository patientRepository;

    public PatientService(PatientRepository patientRepository) {
        this.patientRepository = patientRepository;
    }

    public Patient create(String name, String address, String conditions, String diagnosis) {
        if (name == null || name.isBlank()) {
            throw new IllegalArgumentException("Patient name is required");
        }
        return patientRepository.create(name, address, conditions, diagnosis);
    }

    public Patient get(long id) {
        return patientRepository.require(id);
    }

    public List<Patient> list() {
        return patientRepository.findAllSortedById();
    }

    public int count() {
        return patientRepository.count();
    }

    public void delete(long id) {
        patientRepository.delete(id);
    }
}
